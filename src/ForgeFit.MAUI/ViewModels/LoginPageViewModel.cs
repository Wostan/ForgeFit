// ReSharper disable once RedundantUsingDirective

using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Auth;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.Views;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    [ObservableProperty] private string? _email;
    [ObservableProperty] private string? _password;

    [ObservableProperty] private bool _isEmailError;
    [ObservableProperty] private bool _isEmptyPasswordField;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsNotLoading))]
    private bool _isLoading;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsError))]
    private LocalizedString? _error;

    [ObservableProperty] private LanguageItem? _selectedLanguage;
    
    private readonly IAuthService _authService;
    private readonly IAlertService _alertService;
    private readonly ILocalizationResourceManager _localizationManager;

    public LoginPageViewModel(IAuthService authService, IAlertService alertService, ILocalizationResourceManager localizationManager)
    {
        _authService = authService;
        _alertService = alertService;
        _localizationManager = localizationManager;
        
        var currentCode = _localizationManager.CurrentCulture.TwoLetterISOLanguageName;
        SelectedLanguage = Languages.FirstOrDefault(l => l.Code == currentCode) 
                           ?? Languages.FirstOrDefault(l => l.Code == "en");
    }
    
    public bool IsError => !string.IsNullOrWhiteSpace(Error.Localized);
    public bool IsNotLoading => !IsLoading;

    public record LanguageItem(string Name, string Code);
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
            if (!emailRegex.IsMatch(Email!))
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

            await Shell.Current.GoToAsync($"///{nameof(DiaryPageView)}", false);
        }
        catch (Exception)
        {
            var error = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]).Localized;
            await _alertService.ShowErrorAsync(error, "OK");
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

    [GeneratedRegex("[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$")]
    private static partial Regex EmailRegex();
}
