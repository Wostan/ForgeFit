using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Auth;
using ForgeFit.MAUI.Models.DTOs.Goal;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;
using ForgeFit.MAUI.Services.Interfaces;
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
    private readonly IGoalRealismValidator _goalValidator;
    private readonly ILocalizationResourceManager _localizationManager;

    private CancellationTokenSource? _emailCheckCts;

    [ObservableProperty] private bool _isEmailCheckRunning;
    [ObservableProperty] private bool _isEmailVerified;

    // step 1
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private bool _isEmailError;
    [ObservableProperty] private bool _isPasswordError;
    [ObservableProperty] private bool _isConfirmPasswordError;

    // step 2
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private DateTime _birthDate = DateTime.Today.AddYears(-20);
    [ObservableProperty] private Gender _gender = Gender.Male;
    [ObservableProperty] private bool _isUsernameError;
    
    public DateTime MaxDate => DateTime.Today.AddYears(-13);
    public DateTime MinDate => DateTime.Today.AddYears(-100);

    // step 3
    [ObservableProperty] private double _height = 175;
    [ObservableProperty] private double _weight = 75;
    
    [ObservableProperty] private double _bmiValue;
    [ObservableProperty] private LocalizedString? _bmiCategoryText;
    [ObservableProperty] private Color _bmiColor = Colors.Gray;
    [ObservableProperty] private LocalizedString? _bmiDescription;

    // step 4
    [ObservableProperty] private double _targetWeight;
    [ObservableProperty] private double _minTargetWeight = 30;
    [ObservableProperty] private double _maxTargetWeight = 300;
    [ObservableProperty] private string _daysLeftText = string.Empty;
    [ObservableProperty] private DateTime _goalDueDate = DateTime.Today.AddMonths(3);
    [ObservableProperty] private bool _isNoDeadline;
    [ObservableProperty] private bool _isDeadlineActive = true;
    public DateTime MinGoalDate => DateTime.Today.AddDays(7);
    
    // --- Step 5 Properties ---
    [ObservableProperty] private string _commitmentTitle = string.Empty;
    [ObservableProperty] private string _commitmentSubtitle = string.Empty;

    [ObservableProperty] private bool _isMainNavigationVisible = true;
    [ObservableProperty] private int _currentPosition;
    [ObservableProperty] private string _buttonText = string.Empty;
    [ObservableProperty] private double _progress;
    [ObservableProperty] private LoginPageViewModel.LanguageItem? _selectedLanguage;

    public ObservableCollection<string> Steps { get; } =
    [
        "Credentials", "Personal", "Measurements", "Goal", "Commitment"
    ];

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
        _goalValidator = goalRealismValidator;
        _localizationManager = localizationManager;

        var currentCode = _localizationManager.CurrentCulture.TwoLetterISOLanguageName;
        SelectedLanguage = Languages.FirstOrDefault(l => l.Code == currentCode)
                           ?? Languages.FirstOrDefault(l => l.Code == "en");

        RecalculateBmi();
        RecalculateTargetLimits();
        RecalculateDaysLeft();
        UpdateState();
    }

    public List<LoginPageViewModel.LanguageItem> Languages { get; } =
    [
        new("English", "en"),
        new("Українська", "uk")
    ];

    [RelayCommand]
    private void OnEntryChanged(object? obj = null)
    {
        Error = null;
        
        // step 1
        IsEmailError = false;
        IsPasswordError = false;
        IsConfirmPasswordError = false;
        
        // step 2 
        IsUsernameError = false;
    }
    
    partial void OnHeightChanged(double value)
    {
        var rounded = Math.Round(value, 0, MidpointRounding.AwayFromZero);

        if (Math.Abs(Height - rounded) > 0.01)
        {
            Height = rounded;
        }
    
        RecalculateBmi();
        RecalculateTargetLimits();
    }

    partial void OnWeightChanged(double value)
    {
        var rounded = Math.Round(value, 0, MidpointRounding.AwayFromZero);

        if (Math.Abs(Weight - rounded) > 0.01)
        {
            Weight = rounded;
        }
    
        RecalculateBmi();
    }
    
    private void RecalculateTargetLimits()
    {
        if (Height <= 0) return;

        var heightM = Height / 100.0;
        var heightSq = heightM * heightM;
        
        MinTargetWeight = Math.Ceiling(18.5 * heightSq); 
        MaxTargetWeight = Math.Floor(30.0 * heightSq);   

        if (TargetWeight < MinTargetWeight || TargetWeight > MaxTargetWeight || TargetWeight == 0)
        {
            if (Weight >= MinTargetWeight && Weight <= MaxTargetWeight)
            {
                TargetWeight = Weight;
            }
            else
            {
                TargetWeight = Math.Clamp(Weight, MinTargetWeight, MaxTargetWeight);
            }
        }
        else
        {
            OnPropertyChanged(nameof(TargetWeight));
        }
    }
    
    partial void OnGoalDueDateChanged(DateTime value)
    {
        if (!IsNoDeadline) RecalculateDaysLeft();
    }
    
    [RelayCommand]
    private void ToggleDeadline()
    {
        IsNoDeadline = !IsNoDeadline;
    }
    
    partial void OnIsNoDeadlineChanged(bool value)
    {
        IsDeadlineActive = !value;
        RecalculateDaysLeft();
        
        if (IsError) Error = null; 
    }
    
    private void RecalculateDaysLeft()
    {
        if (IsNoDeadline)
        {
            DaysLeftText = string.Format(_localizationManager["Goal_DaysLeft"], "∞");
            return;
        }

        var days = (GoalDueDate - DateTime.Today).TotalDays;
        var intDays = (int)Math.Ceiling(days);
        
        DaysLeftText = string.Format(_localizationManager["Goal_DaysLeft"], intDays);
    }

    partial void OnUsernameChanged(string value)
    {
        UpdateCommitmentText();
    }

    partial void OnGenderChanged(Gender value)
    {
        UpdateCommitmentText();
    }
    
    private void UpdateCommitmentText()
    {
        var name = string.IsNullOrWhiteSpace(Username) ? "User" : Username;
        
        if (Gender == Gender.Male)
        {
            CommitmentTitle = string.Format(_localizationManager["Reg_Commitment_Title_Male"], name);
            CommitmentSubtitle = _localizationManager["Reg_Commitment_Subtitle_Male"];
        }
        else
        {
            CommitmentTitle = string.Format(_localizationManager["Reg_Commitment_Title_Female"], name);
            CommitmentSubtitle = _localizationManager["Reg_Commitment_Subtitle_Female"];
        }
    }
    
    partial void OnCurrentPositionChanged(int value)
    {
        Error = null;
        
        IsEmailError = false;
        IsPasswordError = false;
        IsConfirmPasswordError = false;
        IsUsernameError = false;
        
        UpdateState();

        if (value == 3) OnPropertyChanged(nameof(TargetWeight));
    }

    [RelayCommand]
    private async Task VerifyEmailAsync()
    {
        Error = null;
        IsEmailError = false;
        IsEmailVerified = false;

        if (string.IsNullOrWhiteSpace(Email) || !EmailRegex().IsMatch(Email))
        {
            _emailCheckCts?.Cancel();
            IsEmailCheckRunning = false;
            return;
        }

        _emailCheckCts?.Cancel();
        _emailCheckCts?.Dispose();
        _emailCheckCts = new CancellationTokenSource();
        var token = _emailCheckCts.Token;

        IsEmailCheckRunning = true;

        try
        {
            await Task.Delay(800, token);

            var result = await _authService.CheckEmailAsync(Email);

            if (token.IsCancellationRequested) return;

            if (result is { Success: true })
            {
                var isTaken = result.Data;
                
                if (isTaken)
                {
                    IsEmailError = true;
                    IsEmailVerified = false;
                    Error = new LocalizedString(() => _localizationManager["Reg_EmailTaken"]);
                }
                else
                {
                    IsEmailVerified = true; 
                    IsEmailError = false;
                }
            }
            else
            {
                IsEmailVerified = false;
                var errorMsg = new LocalizedString(() => result.Message);
                await _alertService.ShowToastAsync(errorMsg.Localized);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"VerifyEmail Error: {ex.Message}");
            IsEmailVerified = false;
            await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            if (!token.IsCancellationRequested)
                IsEmailCheckRunning = false;
        }
    }

    [RelayCommand]
    private async Task NextStep()
    {
        switch (CurrentPosition)
        {
            case 0:
                if (!ValidateStep1()) return;
                break;
            case 1:
                if (!ValidateStep2()) return;
                break;
            case 2:
                if (!ValidateStep3()) return;
                break;
            case 3:
                if (!ValidateStep4()) return; 
                break;
            case 4:
                break;
        }

        if (CurrentPosition < Steps.Count - 1)
        {
            CurrentPosition++;
        }
        else
        {
            await SubmitRegistration();
        }
    }

    private bool ValidateStep1()
    {
        if (string.IsNullOrWhiteSpace(Email) || 
            string.IsNullOrWhiteSpace(Password) || 
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            IsEmailError = string.IsNullOrWhiteSpace(Email);
            IsPasswordError = string.IsNullOrWhiteSpace(Password);
            IsConfirmPasswordError = string.IsNullOrWhiteSpace(ConfirmPassword);
            Error = new LocalizedString(() => _localizationManager["EmptyFieldsMessage"]);
            return false;
        }

        if (IsEmailCheckRunning) return false;

        if (!EmailRegex().IsMatch(Email))
        {
            IsEmailError = true;
            Error = new LocalizedString(() => _localizationManager["EmailErrorMessage"]);
            return false;
        }

        if (!IsEmailVerified)
        {
            IsEmailError = true;
            if (string.IsNullOrWhiteSpace(Error?.Localized))
                Error = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]);
            return false;
        }

        if (Password.Length < 6)
        {
            IsPasswordError = true;
            Error = new LocalizedString(() => _localizationManager["Error_PasswordTooShort"]);
            return false;
        }

        if (Password != ConfirmPassword)
        {
            IsConfirmPasswordError = true;
            Error = new LocalizedString(() => _localizationManager["Reg_ConfirmPasswordError"]);
            return false;
        }

        return true;
    }

    private bool ValidateStep2()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            IsUsernameError = true;
            Error = new LocalizedString(() => _localizationManager["EmptyFieldsMessage"]);
            return false;
        }

        if (Username.Length > 20)
        {
            IsUsernameError = true;
            Error = new LocalizedString(() => _localizationManager["Error_UsernameTooLong"]);
            return false;
        }
        
        var minAgeDate = DateTime.Today.AddYears(-13);
        if (BirthDate > minAgeDate)
        {
            Error = new LocalizedString(() => _localizationManager["Error_InvalidAge"]);
            return false;
        }

        return true;
    }
    
    private bool ValidateStep3()
    {
        if (Height > 0 && Weight > 0) return true;
        
        Error = new LocalizedString(() => _localizationManager["EmptyFieldsMessage"]);
        return false;

    }
    
    private bool ValidateStep4()
    {
        if (TargetWeight <= 0)
        {
            Error = new LocalizedString(() => _localizationManager["Error_InvalidWeight"]);
            return false;
        }

        var goalType = _bmiService.DetermineGoalType(Weight, TargetWeight, Height);
        
        DateTime? goalDueDate = IsNoDeadline ? null : GoalDueDate;
        
        var validationResult = _goalValidator.ValidateGoalRealism(
            Weight,
            TargetWeight,
            Height,
            goalDueDate,
            goalType,
            WeightUnit.Kg
        );

        if (validationResult.IsValid) return true;
        
        Error = new LocalizedString(() => validationResult.ErrorMessage);
        return false;
    }

    [RelayCommand]
    private void PreviousStep()
    {
        if (CurrentPosition <= 0) return;
        
        CurrentPosition--;
    }

    private void UpdateState()
    {
        Progress = (double)CurrentPosition / Steps.Count;

        var isLastStep = CurrentPosition == Steps.Count - 1;

        IsMainNavigationVisible = !isLastStep;

        ButtonText = _localizationManager["Action_Next"];

        if (isLastStep)
        {
            UpdateCommitmentText();
        }
    }
    
    private void RecalculateBmi()
    {
        var bmi = _bmiService.CalculateBmi(Weight, Height);
        
        BmiValue = Math.Round(bmi, 1);

        switch (bmi)
        {
            case < 18.5:
                BmiCategoryText = new LocalizedString(() => _localizationManager["BMI_Underweight"]);
                BmiDescription = new LocalizedString(() => _localizationManager["BMI_Desc_Underweight"]);
                BmiColor = (Color)Application.Current?.Resources["BmiUnderweight"]!;
                break;
            case >= 18.5 and < 25:
                BmiCategoryText = new LocalizedString(() => _localizationManager["BMI_Normal"]);
                BmiDescription = new LocalizedString(() => _localizationManager["BMI_Desc_Normal"]);
                BmiColor = (Color)Application.Current?.Resources["BmiNormal"]!;
                break;
            case >= 25 and < 30:
                BmiCategoryText = new LocalizedString(() => _localizationManager["BMI_Overweight"]);
                BmiDescription = new LocalizedString(() => _localizationManager["BMI_Desc_Overweight"]);
                BmiColor = (Color)Application.Current?.Resources["BmiOverweight"]!;
                break;
            default:
                BmiCategoryText = new LocalizedString(() => _localizationManager["BMI_Obesity"]);
                BmiDescription = new LocalizedString(() => _localizationManager["BMI_Desc_Obesity"]);
                BmiColor = (Color)Application.Current?.Resources["BmiObesity"]!;
                break;
        }
    }

    private async Task SubmitRegistration()
    {
        if (IsLoading) return;
        IsLoading = true;
        Error = null;
        
        try
        {
            var goalType = _bmiService.DetermineGoalType(Weight, TargetWeight, Height);

            var signUpRequest = new UserSignUpRequest(
                Email, 
                Password, 
                Username, 
                null,
                BirthDate, 
                Gender, 
                Weight, 
                WeightUnit.Kg, 
                Height, 
                HeightUnit.Cm
            );
            
            var signUpResult = await _authService.SignUpAsync(signUpRequest);

            if (!signUpResult.Success)
            {
                Error = new LocalizedString(() => signUpResult.Message);
                IsLoading = false;
                return;
            }

            DateTime? goalDueDate = IsNoDeadline ? null : GoalDueDate;

            var bodyGoalRequest = new BodyGoalCreateRequest(
                Title: _localizationManager["Reg_InitialBodyGoalTitle"], 
                Description: _localizationManager["Reg_InitialBodyGoalDesc"],
                WeightGoal: TargetWeight,
                WeightUnit: WeightUnit.Kg,
                DueDate: goalDueDate,
                GoalType: goalType
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
    
    [GeneratedRegex("[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$")]
    private static partial Regex EmailRegex();
}
