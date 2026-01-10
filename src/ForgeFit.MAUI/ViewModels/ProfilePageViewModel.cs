using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.Goal;
using ForgeFit.MAUI.Models.DTOs.User;
using ForgeFit.MAUI.Models.Enums.GoalEnums;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;
using ForgeFit.MAUI.Models.Enums.WorkoutEnums;
using ForgeFit.MAUI.Services.Interfaces;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels;

public partial class ProfilePageViewModel : BaseViewModel
{
    private readonly IUserService _userService;
    private readonly IGoalService _goalService;
    private readonly IPlanService _planService;
    private readonly IAuthService _authService;
    private readonly IBmiService _bmiService;
    private readonly IAlertService _alertService;
    private readonly ILocalizationResourceManager _localizationManager;

    private CancellationTokenSource? _cts;
    [ObservableProperty] private bool _isRefreshing;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _avatarUrl = string.Empty;
    [ObservableProperty] private string _birthDate = string.Empty;
    [ObservableProperty] private string _genderEmoji = string.Empty;
    [ObservableProperty] private string _gender = string.Empty;
    [ObservableProperty] private string _weight = string.Empty;
    [ObservableProperty] private WeightUnit _weightUnit;
    [ObservableProperty] private string _height = string.Empty;
    [ObservableProperty] private HeightUnit _heightUnit;

    [ObservableProperty] private string _bodyGoalEmoji = "🎯";
    [ObservableProperty] private string _bodyGoalWeight = string.Empty;
    [ObservableProperty] private string _bodyGoalTitle = string.Empty;
    [ObservableProperty] private string _bodyGoalDescription = string.Empty;
    [ObservableProperty] private string _bodyGoalDueDate = string.Empty;
    [ObservableProperty] private string _bodyGoalType = string.Empty;
    
    [ObservableProperty] private string _nutritionGoalCalories = string.Empty;
    [ObservableProperty] private string _nutritionGoalCarbs = string.Empty;
    [ObservableProperty] private string _nutritionGoalProtein = string.Empty;
    [ObservableProperty] private string _nutritionGoalFat = string.Empty;
    [ObservableProperty] private string _nutritionGoalVolumeMl = string.Empty;

    [ObservableProperty] private string _workoutGoalPerWeek = string.Empty;
    [ObservableProperty] private string _workoutGoalDurationMinutes = string.Empty;
    [ObservableProperty] private string _workoutGoalType = string.Empty;

    [ObservableProperty] private bool _isEditProfilePopupVisible;
    [ObservableProperty] private bool _isEditBodyGoalPopupVisible;
    [ObservableProperty] private bool _isEditNutritionGoalPopupVisible;
    [ObservableProperty] private bool _isEditWorkoutGoalPopupVisible;
    [ObservableProperty] private bool _isChangePasswordPopupVisible;
    [ObservableProperty] private bool _isConfirmationPopupVisible;

    [ObservableProperty] private string? _editUsername;
    [ObservableProperty] private DateTime _editBirthDate;
    [ObservableProperty] private Gender _editGender;
    [ObservableProperty] private string? _editHeight;
    [ObservableProperty] private string? _editCurrentWeight;

    [ObservableProperty] private string? _editTargetWeight;
    [ObservableProperty] private string? _editBodyGoalTitle;
    [ObservableProperty] private string? _editBodyGoalDescription;
    [ObservableProperty] private DateTime? _editBodyGoalDueDate;

    [ObservableProperty] private string? _editCalories;
    [ObservableProperty] private string? _editCarbs;
    [ObservableProperty] private string? _editProtein;
    [ObservableProperty] private string? _editFat;
    [ObservableProperty] private string? _editWater;

    [ObservableProperty] private string? _editWorkoutsPerWeek;
    [ObservableProperty] private string? _editDurationMinutes;
    [ObservableProperty] private WorkoutTypeOption _editWorkoutType;

    [ObservableProperty] private string? _oldPasswordInput;
    [ObservableProperty] private string? _newPasswordInput;

    [ObservableProperty] private string _confirmationTitle = string.Empty;
    [ObservableProperty] private string _confirmationMessage = string.Empty;
    private Func<Task>? _pendingConfirmationAction;

    public ObservableCollection<Gender> Genders { get; } = new(Enum.GetValues<Gender>());
    public ObservableCollection<WorkoutTypeOption> WorkoutTypes { get; } = [];

    private UserProfileDto? _currentUserProfile;
    private BodyGoalResponse? _currentBodyGoal;
    private NutritionGoalResponse? _currentNutritionGoal;
    private WorkoutGoalResponse? _currentWorkoutGoal;

    public ProfilePageViewModel(
        IUserService userService,
        IGoalService goalService,
        IPlanService planService,
        IAuthService authService,
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
        
        foreach (var type in Enum.GetValues<WorkoutType>())
        {
            WorkoutTypes.Add(new WorkoutTypeOption
            {
                Value = type,
                DisplayName = _localizationManager[$"WorkoutType_{type}"]
            });
        }
        
        LoadDataCommand.Execute(null);
    }
    
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
                UpdateProfileState(profileTask.Result.Data);

            if (bodyGoalTask.Result is { Success: true, Data: not null })
                UpdateBodyGoalState(bodyGoalTask.Result.Data);

            if (nutritionGoalTask.Result is { Success: true, Data: not null })
                UpdateNutritionGoalState(nutritionGoalTask.Result.Data);

            if (workoutGoalTask.Result is { Success: true, Data: not null })
                UpdateWorkoutGoalState(workoutGoalTask.Result.Data);
        }
        catch (OperationCanceledException) { }
        catch (Exception)
        {
            if (!token.IsCancellationRequested)
                await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);
        }
    }
    
    private void UpdateProfileState(UserProfileDto profile)
    {
        _currentUserProfile = profile;
        Username = profile.Username;
        AvatarUrl = profile.AvatarUrl ?? "avatar.svg";
        BirthDate = profile.DateOfBirth.ToString("dd.MM.yyyy");
        Gender = _localizationManager[$"Gender_{profile.Gender}"];
        Weight = profile.Weight.ToString("F1");
        WeightUnit = profile.WeightUnit;
        Height = profile.Height.ToString("F0");
        HeightUnit = profile.HeightUnit;

        GenderEmoji = profile.Gender switch
        {
            Models.Enums.ProfileEnums.Gender.Male => "♂️",
            Models.Enums.ProfileEnums.Gender.Female => "♀️",
            _ => "❔"
        };
    }

    private void UpdateBodyGoalState(BodyGoalResponse goal)
    {
        _currentBodyGoal = goal;
        BodyGoalWeight = goal.WeightGoal.ToString("F1");
        BodyGoalTitle = goal.Title;
        BodyGoalDescription = goal.Description ?? string.Empty;
        BodyGoalDueDate = goal.DueDate.HasValue ? goal.DueDate.Value.ToString("dd.MM.yyyy") : "-";
        BodyGoalType = goal.GoalType switch
        {
            GoalType.MuscleGain => _localizationManager["GoalType_MuscleGain"],
            GoalType.FatLoss => _localizationManager["GoalType_FatLoss"],
            GoalType.WeightGain => _localizationManager["GoalType_WeightGain"],
            _ => "Unknown"
        };
        
        BodyGoalEmoji = goal.GoalType switch
        {
            GoalType.MuscleGain => "💪",
            GoalType.FatLoss => "🔥",
            GoalType.WeightGain => "🥘",
            _ => "🎯"
        };
    }

    private void UpdateNutritionGoalState(NutritionGoalResponse goal)
    {
        _currentNutritionGoal = goal;
        NutritionGoalCalories = goal.Calories.ToString();
        NutritionGoalCarbs = goal.Carbs.ToString();
        NutritionGoalProtein = goal.Protein.ToString();
        NutritionGoalFat = goal.Fat.ToString();
        NutritionGoalVolumeMl = goal.WaterGoalMl.ToString();
    }

    private void UpdateWorkoutGoalState(WorkoutGoalResponse goal)
    {
        _currentWorkoutGoal = goal;
        WorkoutGoalPerWeek = goal.WorkoutsPerWeek.ToString();
        WorkoutGoalDurationMinutes = goal.Duration.TotalMinutes.ToString("F0");
        WorkoutGoalType = _localizationManager[$"WorkoutType_{goal.WorkoutType}"];
    }

    [RelayCommand]
    private void EditProfile()
    {
        if (_currentUserProfile == null) return;
        EditUsername = _currentUserProfile.Username;
        EditBirthDate = _currentUserProfile.DateOfBirth;
        EditGender = _currentUserProfile.Gender;
        EditHeight = _currentUserProfile.Height.ToString(CultureInfo.InvariantCulture);
        EditCurrentWeight = _currentUserProfile.Weight.ToString(CultureInfo.InvariantCulture);
        IsEditProfilePopupVisible = true;
    }

    [RelayCommand]
    private void CloseEditProfile() => IsEditProfilePopupVisible = false;

    [RelayCommand]
    private async Task SaveProfile()
    {
        if (string.IsNullOrWhiteSpace(EditUsername) || 
            !double.TryParse(EditHeight, CultureInfo.InvariantCulture, out var height) ||
            !double.TryParse(EditCurrentWeight, CultureInfo.InvariantCulture, out var weight))
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_InvalidInput"]);
            return;
        }

        if (EditUsername.Length > 20)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_UsernameTooLong"]);
            return;
        }

        var age = DateTime.Today.Year - EditBirthDate.Year;
        if (EditBirthDate.Date > DateTime.Today.AddYears(-age)) age--;
        if (age is < 13 or > 100)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_InvalidAge"]);
            return;
        }

        var wUnit = _currentUserProfile?.WeightUnit ?? WeightUnit.Kg;
        var isWeightValid = wUnit == WeightUnit.Kg 
            ? weight is >= 30 and <= 300 
            : weight is >= 66 and <= 660;
        
        if (!isWeightValid)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_InvalidWeightRange"]);
            return;
        }

        var hUnit = _currentUserProfile?.HeightUnit ?? HeightUnit.Cm;
        var isHeightValid = hUnit == HeightUnit.Cm 
            ? height is >= 100 and <= 250 
            : height is >= 40 and <= 98;
            
        if (!isHeightValid)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_InvalidHeightRange"]);
            return;
        }

        IsEditProfilePopupVisible = false;
        IsLoading = true;

        try
        {
            var dto = new UserProfileDto(
                EditUsername,
                _currentUserProfile?.AvatarUrl,
                EditBirthDate,
                EditGender,
                weight,
                wUnit,
                height,
                hUnit
            );

            var result = await _userService.UpdateProfileAsync(dto, CancellationToken.None);

            if (result is { Success: true, Data: not null })
            {
                UpdateProfileState(result.Data);
                await _alertService.ShowToastAsync(_localizationManager["Success_ProfileUpdated"]);
                
                if (_currentBodyGoal != null)
                {
                    var newGoalType = _bmiService.DetermineGoalType(
                        result.Data.Weight,
                        _currentBodyGoal.WeightGoal,
                        result.Data.Height
                    );

                    if (newGoalType != _currentBodyGoal.GoalType)
                    {
                        var goalRequest = new BodyGoalCreateRequest(
                            _currentBodyGoal.Title,
                            _currentBodyGoal.Description,
                            _currentBodyGoal.WeightGoal,
                            _currentBodyGoal.WeightUnit,
                            _currentBodyGoal.DueDate,
                            newGoalType
                        );

                        var goalUpdateResult = await _goalService.UpdateBodyGoal(goalRequest, CancellationToken.None);
                        if (goalUpdateResult is { Success: true, Data: not null })
                        {
                            UpdateBodyGoalState(goalUpdateResult.Data);
                        }
                    }
                }
            }
            else
            {
                await _alertService.ShowToastAsync(result.Message);
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

    [RelayCommand]
    private void OpenEditBodyGoal()
    {
        if (_currentBodyGoal == null) return;
        
        EditTargetWeight = _currentBodyGoal.WeightGoal.ToString(CultureInfo.InvariantCulture);
        EditBodyGoalTitle = _currentBodyGoal.Title;
        EditBodyGoalDescription = _currentBodyGoal.Description;
        EditBodyGoalDueDate = _currentBodyGoal.DueDate;

        IsEditBodyGoalPopupVisible = true;
    }

    [RelayCommand]
    private void CloseEditBodyGoal() => IsEditBodyGoalPopupVisible = false;

    [RelayCommand]
    private async Task SaveBodyGoal()
    {
        if (!double.TryParse(EditTargetWeight, CultureInfo.InvariantCulture, out var targetWeight))
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_InvalidInput"]);
            return;
        }

        if (string.IsNullOrWhiteSpace(EditBodyGoalTitle))
        {
             await _alertService.ShowToastAsync(_localizationManager["Error_TitleRequired"]);
             return;
        }
        if (EditBodyGoalTitle.Length > 20)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_TitleTooLong"]);
            return;
        }
        if (!string.IsNullOrEmpty(EditBodyGoalDescription) && EditBodyGoalDescription.Length > 200)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_DescriptionTooLong"]);
            return;
        }
        
        var wUnit = _currentBodyGoal?.WeightUnit ?? WeightUnit.Kg;
        var isWeightValid = wUnit == WeightUnit.Kg 
            ? targetWeight is >= 30 and <= 300 
            : targetWeight is >= 66 and <= 660;

        if (!isWeightValid)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_InvalidWeightRange"]);
            return;
        }

        var userHeight = _currentUserProfile?.Height ?? 175;
        var currentWeight = _currentUserProfile?.Weight ?? targetWeight; 
        
        var newGoalType = _bmiService.DetermineGoalType(currentWeight, targetWeight, userHeight);

        if (!await ValidateGoalRealism(currentWeight, targetWeight, EditBodyGoalDueDate, newGoalType, wUnit))
        {
            return; 
        }

        IsEditBodyGoalPopupVisible = false;
        IsLoading = true;

        try
        {
            var request = new BodyGoalCreateRequest(
                EditBodyGoalTitle,
                EditBodyGoalDescription,
                targetWeight,
                wUnit,
                EditBodyGoalDueDate,
                newGoalType
            );

            var result = await _goalService.UpdateBodyGoal(request, CancellationToken.None);

            if (result is { Success: true, Data: not null })
            {
                UpdateBodyGoalState(result.Data);
                await _alertService.ShowToastAsync(_localizationManager["Success_GoalUpdated"]);

                await ExecuteUpdatePlanAsync();
            }
            else
            {
                await _alertService.ShowToastAsync(result.Message);
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
    
    private async Task<bool> ValidateGoalRealism(double currentWeight, double targetWeight, DateTime? dueDate, GoalType type, WeightUnit unit)
    {
        var currentKg = unit == WeightUnit.Kg ? currentWeight : currentWeight * 0.453592;
        var targetKg = unit == WeightUnit.Kg ? targetWeight : targetWeight * 0.453592;

        if (!dueDate.HasValue) return true;

        var days = (dueDate.Value - DateTime.UtcNow).TotalDays;
        
        if (days < 7)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_DeadlineTooClose"]); 
            return false;
        }

        var weeks = days / 7.0;
        var ratePerWeek = Math.Abs(targetKg - currentKg) / weeks;

        switch (type)
        {
            case GoalType.FatLoss when ratePerWeek > 1.3:
            {
                var msg = string.Format(_localizationManager["Error_FatLossUnsafe"], ratePerWeek.ToString("F1"));
                await _alertService.ShowToastAsync(msg);
                return false;
            }
            case GoalType.MuscleGain when ratePerWeek > 0.6:
            {
                var msg = string.Format(_localizationManager["Error_MuscleGainUnrealistic"], ratePerWeek.ToString("F1"));
                await _alertService.ShowToastAsync(msg);
                return false;
            }
            default:
                return true;
        }
    }

    [RelayCommand]
    private void OpenEditNutritionGoal()
    {
        if (_currentNutritionGoal == null) return;
        EditCalories = _currentNutritionGoal.Calories.ToString();
        EditCarbs = _currentNutritionGoal.Carbs.ToString();
        EditProtein = _currentNutritionGoal.Protein.ToString();
        EditFat = _currentNutritionGoal.Fat.ToString();
        EditWater = _currentNutritionGoal.WaterGoalMl.ToString();
        IsEditNutritionGoalPopupVisible = true;
    }

    [RelayCommand]
    private void CloseEditNutrition() => IsEditNutritionGoalPopupVisible = false;

    [RelayCommand]
    private async Task SaveNutrition()
    {
        if (!int.TryParse(EditCalories, out var cal) || !int.TryParse(EditCarbs, out var carbs) ||
            !int.TryParse(EditProtein, out var prot) || !int.TryParse(EditFat, out var fat) ||
            !int.TryParse(EditWater, out var water))
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_InvalidInput"]);
            return;
        }

        if (cal is < 500 or > 10000)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_CaloriesRange"]);
            return;
        }
        if (carbs < 0 || prot < 0 || fat < 0)
        {
             await _alertService.ShowToastAsync(_localizationManager["Error_MacrosPositive"]);
             return;
        }
        if (water is < 1000 or > 10000)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_WaterRange"]);
            return;
        }

        IsEditNutritionGoalPopupVisible = false;
        IsLoading = true;

        try
        {
            var request = new NutritionGoalCreateRequest(cal, carbs, prot, fat, water);
            var result = await _goalService.UpdateNutritionGoal(request);

            if (result is { Success: true, Data: not null })
            {
                UpdateNutritionGoalState(result.Data);
                await _alertService.ShowToastAsync(_localizationManager["Success_NutritionUpdated"]);
            }
            else
            {
                await _alertService.ShowToastAsync(result.Message);
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

    [RelayCommand]
    private void OpenEditWorkoutGoal()
    {
        if (_currentWorkoutGoal == null) return;
        EditWorkoutsPerWeek = _currentWorkoutGoal.WorkoutsPerWeek.ToString();
        EditDurationMinutes = _currentWorkoutGoal.Duration.TotalMinutes.ToString("F0");
        EditWorkoutType = WorkoutTypes.FirstOrDefault(x => x.Value == _currentWorkoutGoal.WorkoutType) ?? new WorkoutTypeOption();
        IsEditWorkoutGoalPopupVisible = true;
    }

    [RelayCommand]
    private void CloseEditWorkout() => IsEditWorkoutGoalPopupVisible = false;

    [RelayCommand]
    private async Task SaveWorkout()
    {
        if (!int.TryParse(EditWorkoutsPerWeek, out var freq) || 
            !double.TryParse(EditDurationMinutes, CultureInfo.InvariantCulture, out var duration))
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_InvalidInput"]);
            return;
        }

        if (freq is < 1 or > 7)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_WorkoutFrequency"]);
            return;
        }
        
        if (duration is < 5 or > 300)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_WorkoutDuration"]);
            return;
        }

        IsEditWorkoutGoalPopupVisible = false;
        IsLoading = true;

        try
        {
            var request = new WorkoutGoalCreateRequest(freq, TimeSpan.FromMinutes(duration), EditWorkoutType.Value);
            var result = await _goalService.UpdateWorkoutGoal(request, CancellationToken.None);

            if (result is { Success: true, Data: not null })
            {
                UpdateWorkoutGoalState(result.Data);
                await _alertService.ShowToastAsync(_localizationManager["Success_WorkoutUpdated"]);
            }
            else
            {
                await _alertService.ShowToastAsync(result.Message);
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

    [RelayCommand]
    private void OpenChangePassword()
    {
        OldPasswordInput = string.Empty;
        NewPasswordInput = string.Empty;
        IsChangePasswordPopupVisible = true;
    }

    [RelayCommand]
    private void ClosePasswordPopup() => IsChangePasswordPopupVisible = false;

    [RelayCommand]
    private async Task SavePassword()
    {
        if (string.IsNullOrWhiteSpace(OldPasswordInput))
        {
             await _alertService.ShowToastAsync(_localizationManager["Error_CurrentPasswordRequired"]);
             return;
        }
        if (string.IsNullOrWhiteSpace(NewPasswordInput) || NewPasswordInput.Length < 6)
        {
             await _alertService.ShowToastAsync(_localizationManager["Error_PasswordTooShort"]);
             return;
        }
        if (OldPasswordInput == NewPasswordInput)
        {
             await _alertService.ShowToastAsync(_localizationManager["Error_NewPasswordSame"]);
             return;
        }

        IsChangePasswordPopupVisible = false;
        IsLoading = true;

        try
        {
            var request = new ChangePasswordRequest(OldPasswordInput, NewPasswordInput);
            var result = await _userService.ChangePasswordAsync(request);

            if (result.Success)
            {
                await _alertService.ShowToastAsync(_localizationManager["Success_PasswordChanged"]);
            }
            else
            {
                await _alertService.ShowToastAsync(result.Message);
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

    [RelayCommand]
    private void UpdatePlan()
    {
        ConfirmationTitle = _localizationManager["Confirm_UpdatePlan_Title"];
        ConfirmationMessage = _localizationManager["Confirm_UpdatePlan_Message"];
        _pendingConfirmationAction = ExecuteUpdatePlanAsync;
        IsConfirmationPopupVisible = true;
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
                UpdateBodyGoalState(getPlanResult.Data.BodyGoal);
                UpdateNutritionGoalState(getPlanResult.Data.NutritionGoal);
                UpdateWorkoutGoalState(getPlanResult.Data.WorkoutGoal);
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

    [RelayCommand]
    private void Logout()
    {
        ConfirmationTitle = _localizationManager["Confirm_Logout_Title"];
        ConfirmationMessage = _localizationManager["Confirm_Logout_Message"];
        _pendingConfirmationAction = ExecuteLogoutAsync;
        IsConfirmationPopupVisible = true;
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

    [RelayCommand]
    private void CloseConfirmationPopup()
    {
        IsConfirmationPopupVisible = false;
        _pendingConfirmationAction = null;
    }

    [RelayCommand]
    private async Task ConfirmAction()
    {
        IsConfirmationPopupVisible = false;
        if (_pendingConfirmationAction != null)
        {
            await _pendingConfirmationAction.Invoke();
            _pendingConfirmationAction = null;
        }
    }
}

public class WorkoutTypeOption
{
    public WorkoutType Value { get; set; }
    public string DisplayName { get; set; }
}
