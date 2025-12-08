// ReSharper disable once RedundantUsingDirective

using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Auth;
using ForgeFit.MAUI.Resources.Strings;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.Views;

namespace ForgeFit.MAUI.ViewModels;

public partial class LoginPageViewModel(IAuthService authService, IAlertService alertService) : ObservableObject
{
    [ObservableProperty] private string? _email;

    [ObservableProperty] private string? _password;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsError))]
    private string? _error = string.Empty;

    [ObservableProperty] private bool _isEmptyEmailField;

    [ObservableProperty] private bool _isEmptyPasswordField;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsNotLoading))]
    private bool _isLoading;

    public bool IsError => !string.IsNullOrWhiteSpace(Error);
    public bool IsNotLoading => !IsLoading;

    [RelayCommand]
    private void OnEntryChanged()
    {
        Error = null;
        IsEmptyEmailField = false;
        IsEmptyPasswordField = false;
    }

    [RelayCommand]
    private async Task SignInAsync()
    {
        if (IsLoading) return;

        try
        {
            Error = null;
            var isEmptyEntry = false;

            if (string.IsNullOrWhiteSpace(Email))
            {
                IsEmptyEmailField = true;
                isEmptyEntry = true;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                IsEmptyPasswordField = true;
                isEmptyEntry = true;
            }

            if (isEmptyEntry)
            {
                Error = AppResources.EmptyFieldsMessage;
                return;
            }

            IsLoading = true;
            var result = await authService.SignInAsync(new UserSignInRequest(Email!, Password!));
            var isSuccess = result.Data;

            if (!isSuccess)
            {
                Error = result.Message;
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(DiaryPageView)}");
        }
        catch (Exception)
        {
            await alertService.ShowErrorAsync(AppResources.UnexpectedErrorMessage, "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
