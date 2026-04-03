using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Handlers;
using ForgeFit.MAUI.Models.DTOs.Auth;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using ForgeFit.MAUI.Views.Auth;
using LocalizationResourceManager.Maui;

// ReSharper disable once RedundantUsingDirective

namespace ForgeFit.MAUI.ViewModels.Auth;

public partial class LoginPageViewModel : BaseViewModel
{
    private readonly IAlertService _alertService;
    private readonly IAuthService _authService;
    private readonly ILocalizationResourceManager _localizationManager;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty] private string? _email;

    [ObservableProperty] private bool _isEmailError;
    [ObservableProperty] private bool _isEmptyPasswordField;
    [ObservableProperty] private string? _password;

    [ObservableProperty] private LanguageItem? _selectedLanguage;

    public LoginPageViewModel(
        IAuthService authService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager,
        IServiceProvider serviceProvider)
    {
        _authService = authService;
        _alertService = alertService;
        _localizationManager = localizationManager;
        _serviceProvider = serviceProvider;


        var currentCode = _localizationManager.CurrentCulture.TwoLetterISOLanguageName;
        SelectedLanguage = Languages.FirstOrDefault(l => l.Code == currentCode)
                           ?? Languages.FirstOrDefault(l => l.Code == "en");
    }

    public bool IsNotLoading => !IsLoading;

    public List<LanguageItem> Languages { get; } =
    [
        new("English", "en"),
        new("Українська", "uk")
    ];

    [RelayCommand]
    private void OnEntryChanged()
    {
        Error = null;
        IsEmailError = false;
        IsEmptyPasswordField = false;
    }

    [RelayCommand]
    private async Task SignInAsync()
    {
        if (IsLoading) return;

        try
        {
            Error = new LocalizedString(() => string.Empty);
            var isEmptyEntry = false;

            if (string.IsNullOrWhiteSpace(Email))
            {
                IsEmailError = true;
                isEmptyEntry = true;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                IsEmptyPasswordField = true;
                isEmptyEntry = true;
            }

            if (isEmptyEntry)
            {
                Error = new LocalizedString(() => _localizationManager["EmptyFieldsMessage"]);
                return;
            }

            var emailRegex = EmailRegex();
            if (!emailRegex.IsMatch((string)Email!))
            {
                IsEmailError = true;
                Error = new LocalizedString(() => _localizationManager["EmailErrorMessage"]);
                return;
            }

            IsLoading = true;
            var result = await _authService.SignInAsync(new UserSignInRequest(Email!, Password!));
            var isSuccess = result.Data;

            if (!isSuccess)
            {
                Error = new LocalizedString(() => result.Message);
                return;
            }

            RefreshTokenHandler.ResetState();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Application.Current != null && Application.Current.Windows.Count > 0)
                    Application.Current.Windows[0].Page = new AppShell();
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception in SignInAsync: " + ex.Message);
            var error = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]).Localized;
            await _alertService.ShowToastAsync(error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedLanguageChanged(LanguageItem? value)
    {
        if (value is null) return;
        _localizationManager.CurrentCulture = new CultureInfo(value.Code);
    }

    [RelayCommand]
    private async Task GoToRegistration()
    {
        try
        {
            var registrationPage = _serviceProvider.GetRequiredService<RegistrationPageView>();
            Application.Current!.Windows[0].Page = registrationPage;
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

    public record LanguageItem(string Name, string Code);
}