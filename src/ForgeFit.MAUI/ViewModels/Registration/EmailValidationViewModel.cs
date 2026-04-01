using System.Diagnostics;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Services.Interfaces;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Registration;

public partial class EmailValidationViewModel(
    IAuthService authService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager)
    : BaseViewModel
{
    private CancellationTokenSource? _emailCheckCts;

    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private bool _isEmailError;
    [ObservableProperty] private bool _isPasswordError;
    [ObservableProperty] private bool _isConfirmPasswordError;
    [ObservableProperty] private bool _isEmailCheckRunning;
    [ObservableProperty] private bool _isEmailVerified;

    [RelayCommand]
    public void OnEntryChanged(object? obj = null)
    {
        IsEmailError = false;
        IsPasswordError = false;
        IsConfirmPasswordError = false;
    }

    [RelayCommand]
    private async Task VerifyEmailAsync()
    {
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

            var result = await authService.CheckEmailAsync(Email);

            if (token.IsCancellationRequested) return;

            if (result is { Success: true })
            {
                var isTaken = result.Data;

                if (isTaken)
                {
                    IsEmailError = true;
                    IsEmailVerified = false;
                    ValidationError = new LocalizedString(() => localizationManager["Reg_EmailTaken"]);
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
                await alertService.ShowToastAsync(errorMsg.Localized);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"VerifyEmail Error: {ex.Message}");
            IsEmailVerified = false;
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            if (!token.IsCancellationRequested)
                IsEmailCheckRunning = false;
        }
    }

    public bool ValidateStep()
    {
        if (string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            IsEmailError = string.IsNullOrWhiteSpace(Email);
            IsPasswordError = string.IsNullOrWhiteSpace(Password);
            IsConfirmPasswordError = string.IsNullOrWhiteSpace(ConfirmPassword);
            ValidationError = new LocalizedString(() => localizationManager["EmptyFieldsMessage"]);
            return false;
        }

        if (IsEmailCheckRunning) return false;

        if (!EmailRegex().IsMatch(Email))
        {
            IsEmailError = true;
            ValidationError = new LocalizedString(() => localizationManager["EmailErrorMessage"]);
            return false;
        }

        if (!IsEmailVerified)
        {
            IsEmailError = true;
            if (string.IsNullOrWhiteSpace(ValidationError?.Localized))
                ValidationError = new LocalizedString(() => localizationManager["UnexpectedErrorMessage"]);
            return false;
        }

        if (Password.Length < 6)
        {
            IsPasswordError = true;
            ValidationError = new LocalizedString(() => localizationManager["Error_PasswordTooShort"]);
            return false;
        }

        if (Password != ConfirmPassword)
        {
            IsConfirmPasswordError = true;
            ValidationError = new LocalizedString(() => localizationManager["Reg_ConfirmPasswordError"]);
            return false;
        }

        return true;
    }

    public void ClearErrors()
    {
        IsEmailError = false;
        IsPasswordError = false;
        IsConfirmPasswordError = false;
        ValidationError = null;
    }

    [ObservableProperty] private LocalizedString? _validationError;

    [GeneratedRegex("[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$")]
    private static partial Regex EmailRegex();
}
