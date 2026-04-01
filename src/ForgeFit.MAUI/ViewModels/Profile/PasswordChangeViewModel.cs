using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Auth;
using ForgeFit.MAUI.Models.DTOs.User;
using ForgeFit.MAUI.Services.Interfaces;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Profile;

public partial class PasswordChangeViewModel(
    IUserService userService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager)
    : BaseViewModel
{
    [ObservableProperty] private bool _isChangePasswordPopupVisible;
    [ObservableProperty] private string? _oldPasswordInput;
    [ObservableProperty] private string? _newPasswordInput;

    [RelayCommand]
    private void OpenChangePassword()
    {
        OldPasswordInput = string.Empty;
        NewPasswordInput = string.Empty;
        IsChangePasswordPopupVisible = true;
    }

    [RelayCommand]
    private void ClosePasswordPopup()
    {
        IsChangePasswordPopupVisible = false;
    }

    [RelayCommand]
    private async Task SavePassword()
    {
        if (string.IsNullOrWhiteSpace(OldPasswordInput))
        {
            await alertService.ShowToastAsync(localizationManager["Error_CurrentPasswordRequired"]);
            return;
        }

        if (string.IsNullOrWhiteSpace(NewPasswordInput) || NewPasswordInput.Length < 6)
        {
            await alertService.ShowToastAsync(localizationManager["Error_PasswordTooShort"]);
            return;
        }

        if (OldPasswordInput == NewPasswordInput)
        {
            await alertService.ShowToastAsync(localizationManager["Error_NewPasswordSame"]);
            return;
        }

        IsChangePasswordPopupVisible = false;
        IsLoading = true;

        try
        {
            var request = new ChangePasswordRequest(OldPasswordInput, NewPasswordInput);
            var result = await userService.ChangePasswordAsync(request);

            if (result.Success)
                await alertService.ShowToastAsync(localizationManager["Success_PasswordChanged"]);
            else
                await alertService.ShowToastAsync(result.Message);
        }
        catch
        {
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
