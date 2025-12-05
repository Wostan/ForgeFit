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

    [ObservableProperty] private string? _error = string.Empty;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsNotLoading))]
    private bool _isLoading;

    public bool IsError => !string.IsNullOrWhiteSpace(Error);
    public bool IsNotLoading => !IsLoading;

    [RelayCommand]
    private void OnEntryChanged()
    {
        if (IsError)
            Error = null;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                Error = AppResources.EmptyFieldsMessage;
                return;
            }

            var result =
                await authService.SignInAsync(new UserSignInRequest(Email, Password));

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
