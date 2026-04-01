using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Auth;
using ForgeFit.MAUI.Models.DTOs.Goal;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Registration;
using ForgeFit.MAUI.Views.Auth;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels;

public partial class RegistrationPageViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IAlertService _alertService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IBmiService _bmiService;
    private readonly IGoalService _goalService;
    private readonly IPlanService _planService;
    private readonly ILocalizationResourceManager _localizationManager;

    public EmailValidationViewModel EmailVM { get; }
    public PersonalInfoViewModel PersonalVM { get; }
    public MeasurementsViewModel MeasurementsVM { get; }
    public GoalSettingsViewModel GoalVM { get; }
    public NavigationViewModel NavigationVM { get; }

    public RegistrationPageViewModel(
        IAuthService authService,
        IAlertService alertService,
        IServiceProvider serviceProvider,
        IBmiService bmiService,
        IGoalService goalService,
        IPlanService planService,
        IGoalRealismValidator goalRealismValidator,
        ILocalizationResourceManager localizationManager)
    {
        _authService = authService;
        _alertService = alertService;
        _serviceProvider = serviceProvider;
        _bmiService = bmiService;
        _goalService = goalService;
        _planService = planService;
        _localizationManager = localizationManager;

        EmailVM = new EmailValidationViewModel(authService, alertService, localizationManager);
        PersonalVM = new PersonalInfoViewModel(localizationManager);
        MeasurementsVM = new MeasurementsViewModel(bmiService, localizationManager);
        GoalVM = new GoalSettingsViewModel(bmiService, goalRealismValidator, localizationManager);
        NavigationVM = new NavigationViewModel(localizationManager);

        PersonalVM.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(PersonalVM.Username) or nameof(PersonalVM.Gender))
            {
                NavigationVM.UpdateCommitmentText(PersonalVM.Username, PersonalVM.Gender);
            }
        };

        MeasurementsVM.TargetLimitsChanged += () =>
        {
            GoalVM.SetCurrentMeasurements(MeasurementsVM.Height, MeasurementsVM.Weight);
        };

        GoalVM.SetCurrentMeasurements(MeasurementsVM.Height, MeasurementsVM.Weight);
    }

    [RelayCommand]
    private void OnEntryChanged(object? obj = null)
    {
        Error = null;
        EmailVM.ClearErrors();
        PersonalVM.ClearErrors();
        MeasurementsVM.ClearErrors();
        GoalVM.ClearErrors();
    }

    [RelayCommand]
    private async Task NextStep()
    {
        switch (NavigationVM.CurrentPosition)
        {
            case 0:
                if (!EmailVM.ValidateStep()) return;
                break;
            case 1:
                if (!PersonalVM.ValidateStep()) return;
                break;
            case 2:
                if (!MeasurementsVM.ValidateStep()) return;
                break;
            case 3:
                if (!GoalVM.ValidateStep()) return;
                break;
            case 4:
                break;
        }

        if (NavigationVM.CurrentPosition < NavigationVM.Steps.Count - 1)
            NavigationVM.CurrentPosition++;
        else
            await SubmitRegistration();
    }

    [RelayCommand]
    private void PreviousStep()
    {
        if (NavigationVM.CurrentPosition <= 0) return;

        NavigationVM.CurrentPosition--;
    }

    private async Task SubmitRegistration()
    {
        if (IsLoading) return;
        IsLoading = true;
        Error = null;

        try
        {
            var goalType = _bmiService.DetermineGoalType(MeasurementsVM.Weight, GoalVM.TargetWeight, MeasurementsVM.Height);

            var signUpRequest = new UserSignUpRequest(
                EmailVM.Email,
                EmailVM.Password,
                PersonalVM.Username,
                null,
                PersonalVM.BirthDate,
                PersonalVM.Gender,
                MeasurementsVM.Weight,
                WeightUnit.Kg,
                MeasurementsVM.Height,
                HeightUnit.Cm
            );

            var signUpResult = await _authService.SignUpAsync(signUpRequest);

            if (!signUpResult.Success)
            {
                Error = new LocalizedString(() => signUpResult.Message);
                IsLoading = false;
                return;
            }

            DateTime? goalDueDate = GoalVM.IsNoDeadline ? null : GoalVM.GoalDueDate;

            var bodyGoalRequest = new BodyGoalCreateRequest(
                _localizationManager["Reg_InitialBodyGoalTitle"],
                _localizationManager["Reg_InitialBodyGoalDesc"],
                GoalVM.TargetWeight,
                WeightUnit.Kg,
                goalDueDate,
                goalType
            );

            var goalResult = await _goalService.CreateBodyGoal(bodyGoalRequest);

            if (!goalResult.Success)
            {
                var errorMsg = new LocalizedString(() => goalResult.Message);
                await _alertService.ShowToastAsync(errorMsg.Localized);
                IsLoading = false;
                return;
            }

            var planResult = await _planService.GeneratePlanAsync();
            if (!planResult.Success || planResult.Data == null)
            {
                var errorMsg = new LocalizedString(() => planResult.Message);
                await _alertService.ShowToastAsync(errorMsg.Localized);
                IsLoading = false;
                return;
            }

            var confirmResult = await _planService.ConfirmPlanAsync(planResult.Data);

            if (confirmResult is { Success: false })
            {
                var errorMsg = new LocalizedString(() => confirmResult.Message);
                await _alertService.ShowToastAsync(errorMsg.Localized);
                IsLoading = false;
                return;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Application.Current != null && Application.Current.Windows.Count > 0)
                    Application.Current.Windows[0].Page = new AppShell();
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Registration Error: {ex}");
            var errorMsg = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]);
            await _alertService.ShowToastAsync(errorMsg.Localized);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GoToLogin()
    {
        try
        {
            var loginPage = _serviceProvider.GetRequiredService<LoginPageView>();
            Application.Current!.Windows[0].Page = loginPage;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception in GoToRegistration: " + ex.Message);
            var errorMsg = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]);
            await _alertService.ShowToastAsync(errorMsg.Localized);
        }
    }
}
