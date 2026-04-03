using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.Goal;
using ForgeFit.MAUI.Models.DTOs.User;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using ForgeFit.MAUI.ViewModels.Profile.Goals;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Profile.Main;

public partial class ProfilePageViewModel : BaseViewModel
{
    private readonly IAlertService _alertService;
    private readonly IAuthService _authService;
    private readonly IBmiService _bmiService;
    private readonly IGoalService _goalService;
    private readonly ILocalizationResourceManager _localizationManager;
    private readonly IPlanService _planService;
    private readonly IUserService _userService;

    private CancellationTokenSource? _cts;

    private BodyGoalResponse? _currentBodyGoal;
    [ObservableProperty] private bool _isRefreshing;

    public ProfilePageViewModel(
        IUserService userService,
        IGoalService goalService,
        IPlanService planService,
        IAuthService authService,
        IGoalRealismValidator goalRealismValidator,
        IBmiService bmiService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager)
    {
        _userService = userService;
        _goalService = goalService;
        _planService = planService;
        _authService = authService;
        _bmiService = bmiService;
        _alertService = alertService;
        _localizationManager = localizationManager;

        UserProfileVM = new UserProfileViewModel(userService, alertService, localizationManager);
        BodyGoalVM = new BodyGoalViewModel(goalService, bmiService, goalRealismValidator, alertService,
            localizationManager);
        NutritionGoalVM = new NutritionGoalViewModel(goalService, alertService, localizationManager);
        WorkoutGoalVM = new WorkoutGoalViewModel(goalService, alertService, localizationManager);
        PasswordChangeVM = new PasswordChangeViewModel(userService, alertService, localizationManager);
        ConfirmationVM = new ConfirmationViewModel(localizationManager);

        WorkoutGoalVM.InitializeWorkoutTypes();

        UserProfileVM.OnProfileUpdated += OnProfileUpdated;

        BodyGoalVM.OnGoalUpdated += OnBodyGoalUpdated;
        NutritionGoalVM.OnGoalUpdated += OnManualGoalUpdated;
        WorkoutGoalVM.OnGoalUpdated += OnManualGoalUpdated;

        LoadDataCommand.Execute(null);
    }

    public UserProfileViewModel UserProfileVM { get; }
    public BodyGoalViewModel BodyGoalVM { get; }
    public NutritionGoalViewModel NutritionGoalVM { get; }
    public WorkoutGoalViewModel WorkoutGoalVM { get; }
    public PasswordChangeViewModel PasswordChangeVM { get; }
    public ConfirmationViewModel ConfirmationVM { get; }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            await FetchDataInternal();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        try
        {
            await FetchDataInternal();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async Task FetchDataInternal()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            var profileTask = _userService.GetProfileAsync(token);
            var bodyGoalTask = _goalService.GetBodyGoal(token);
            var nutritionGoalTask = _goalService.GetNutritionGoal(token);
            var workoutGoalTask = _goalService.GetWorkoutGoal(token);

            await Task.WhenAll(profileTask, bodyGoalTask, nutritionGoalTask, workoutGoalTask);

            if (token.IsCancellationRequested) return;

            if (profileTask.Result is { Success: true, Data: not null })
            {
                UserProfileVM.UpdateState(profileTask.Result.Data);
                BodyGoalVM.SetUserProfile(profileTask.Result.Data);
            }

            if (bodyGoalTask.Result is { Success: true, Data: not null })
            {
                _currentBodyGoal = bodyGoalTask.Result.Data;
                BodyGoalVM.UpdateState(bodyGoalTask.Result.Data);
            }

            if (nutritionGoalTask.Result is { Success: true, Data: not null })
            {
                NutritionGoalVM.UpdateState(nutritionGoalTask.Result.Data);
                NutritionGoalVM.SetCurrentGoal(nutritionGoalTask.Result.Data);
            }

            if (workoutGoalTask.Result is { Success: true, Data: not null })
                WorkoutGoalVM.UpdateState(workoutGoalTask.Result.Data);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
            if (!token.IsCancellationRequested)
                await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);
        }
    }

    private void OnProfileUpdated(UserProfileDto updatedProfile)
    {
        BodyGoalVM.SetUserProfile(updatedProfile);

        if (_currentBodyGoal == null) return;

        var newGoalType = _bmiService.DetermineGoalType(
            updatedProfile.Weight,
            _currentBodyGoal.WeightGoal,
            updatedProfile.Height
        );

        if (newGoalType == _currentBodyGoal.GoalType) return;

        var goalRequest = new BodyGoalCreateRequest(
            _currentBodyGoal.Title,
            _currentBodyGoal.Description,
            _currentBodyGoal.WeightGoal,
            _currentBodyGoal.WeightUnit,
            _currentBodyGoal.DueDate,
            newGoalType
        );

        _ = Task.Run(async () =>
        {
            try
            {
                var goalUpdateResult = await _goalService.UpdateBodyGoal(goalRequest, CancellationToken.None);
                if (goalUpdateResult is { Success: true, Data: not null })
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _currentBodyGoal = goalUpdateResult.Data;
                        BodyGoalVM.UpdateState(goalUpdateResult.Data);
                    });
            }
            catch
            {
                // ignored
            }
        });
    }

    private void OnBodyGoalUpdated()
    {
        MainThread.BeginInvokeOnMainThread(async void () => await ExecuteUpdatePlanAsync());
    }

    private void OnManualGoalUpdated()
    {
        MainThread.BeginInvokeOnMainThread(() => { WeakReferenceMessenger.Default.Send(new DiaryUpdatedMessage()); });
    }

    [RelayCommand]
    private void UpdatePlan()
    {
        ConfirmationVM.ShowConfirmation("Confirm_UpdatePlan_Title", "Confirm_UpdatePlan_Message",
            ExecuteUpdatePlanAsync);
    }

    [RelayCommand]
    private void Logout()
    {
        ConfirmationVM.ShowConfirmation("Confirm_Logout_Title", "Confirm_Logout_Message", ExecuteLogoutAsync);
    }

    private async Task ExecuteUpdatePlanAsync()
    {
        IsLoading = true;
        try
        {
            var getPlanResult = await _planService.GeneratePlanAsync(CancellationToken.None);
            if (!getPlanResult.Success || getPlanResult.Data == null)
            {
                await _alertService.ShowToastAsync(getPlanResult.Message);
                return;
            }

            var savePlanResult = await _planService.ConfirmPlanAsync(getPlanResult.Data, CancellationToken.None);

            if (savePlanResult is { Success: true })
            {
                _currentBodyGoal = getPlanResult.Data.BodyGoal;

                BodyGoalVM.UpdateState(getPlanResult.Data.BodyGoal);
                NutritionGoalVM.UpdateState(getPlanResult.Data.NutritionGoal);
                WorkoutGoalVM.UpdateState(getPlanResult.Data.WorkoutGoal);
                WeakReferenceMessenger.Default.Send(new DiaryUpdatedMessage());
            }
            else
            {
                await _alertService.ShowToastAsync(getPlanResult.Message);
            }
        }
        catch
        {
            await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            IsLoading = false;
        }
    }


    private async Task ExecuteLogoutAsync()
    {
        IsLoading = true;
        try
        {
            _authService.SignOut();
        }
        catch
        {
            await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            IsLoading = false;
        }
    }
}