using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Profile.Main;

public partial class ConfirmationViewModel(ILocalizationResourceManager localizationManager) : BaseViewModel
{
    [ObservableProperty] private string _confirmationMessage = string.Empty;
    [ObservableProperty] private string _confirmationTitle = string.Empty;
    [ObservableProperty] private bool _isConfirmationPopupVisible;

    private Func<Task>? _pendingConfirmationAction;

    public void ShowConfirmation(string titleKey, string messageKey, Func<Task> action)
    {
        ConfirmationTitle = localizationManager[titleKey];
        ConfirmationMessage = localizationManager[messageKey];
        _pendingConfirmationAction = action;
        IsConfirmationPopupVisible = true;
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