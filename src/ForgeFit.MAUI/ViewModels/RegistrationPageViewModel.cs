using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    // step 3
    [ObservableProperty] private string _height = string.Empty;
    [ObservableProperty] private string _weight = string.Empty;
    [ObservableProperty] private string _targetWeight = string.Empty;

    public DateTime MaxDate => DateTime.Today.AddYears(-13);
    public DateTime MinDate => DateTime.Today.AddYears(-100);

    [ObservableProperty] private int _currentPosition;
    [ObservableProperty] private string _buttonText = string.Empty;
    [ObservableProperty] private double _progress;
    [ObservableProperty] private LoginPageViewModel.LanguageItem? _selectedLanguage;

    public ObservableCollection<string> Steps { get; } =
    [
        "Credentials", "Personal", "Measurements", "Goal"
    ];

    public RegistrationPageViewModel(
        IAuthService authService,
        IAlertService alertService,
        IServiceProvider serviceProvider,
        ILocalizationResourceManager localizationManager)
    {
        _authService = authService;
        _alertService = alertService;
        _serviceProvider = serviceProvider;
        _localizationManager = localizationManager;

        var currentCode = _localizationManager.CurrentCulture.TwoLetterISOLanguageName;
        SelectedLanguage = Languages.FirstOrDefault(l => l.Code == currentCode)
                           ?? Languages.FirstOrDefault(l => l.Code == "en");

        UpdateState();
        
        //for testing purposes
        CurrentPosition = 1;
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
    private void NextStep()
    {
        switch (CurrentPosition)
        {
            case 0:
                if (!ValidateStep1()) return;
                break;
            case 1:
                if (!ValidateStep2()) return;
                break;
            // case 2:
        }

        if (CurrentPosition < Steps.Count - 1)
        {
            CurrentPosition++;
            UpdateState();
        }
        else
        {
            SubmitRegistration();
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

    [RelayCommand]
    private void PreviousStep()
    {
        if (CurrentPosition <= 0) return;
        
        CurrentPosition--;
        UpdateState();
    }

    private void UpdateState()
    {
        ButtonText = CurrentPosition == Steps.Count - 1
            ? _localizationManager["Action_Finish"]
            : _localizationManager["Action_Next"];

        Progress = (double)CurrentPosition / Steps.Count;
    }

    private static void SubmitRegistration()
    {
        Shell.Current.DisplayAlert("Success", "Registration Logic Here", "OK");
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
