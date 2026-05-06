using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Core;

public partial class PopupManagerViewModel(ILocalizationResourceManager localizationManager) : ObservableObject
{
    [ObservableProperty] private string _confirmationMessage = string.Empty;
    [ObservableProperty] private string _confirmationTitle = string.Empty;
    [ObservableProperty] private string _createInputValue = string.Empty;
    [ObservableProperty] private TimeSpan _entryPopupDuration;
    [ObservableProperty] private string _entryPopupPlaceholder = string.Empty;
    [ObservableProperty] private string _entryPopupTitle = string.Empty;
    [ObservableProperty] private string _tempProgramName = string.Empty;

    [ObservableProperty] private bool _isConfirmationPopupVisible;
    [ObservableProperty] private bool _isCreatePopupVisible;
    [ObservableProperty] private bool _isEntryPopupVisible;
    [ObservableProperty] private bool _isRenamePopupVisible;
    [ObservableProperty] private bool _isCreateFoodPopupVisible;

    private Func<Task>? _pendingConfirmationAction;

    public void ShowConfirmation(string titleKey, string messageKey, Func<Task> confirmationAction)
    {
        ConfirmationTitle = localizationManager[titleKey];
        ConfirmationMessage = localizationManager[messageKey];

        _pendingConfirmationAction = confirmationAction;
        IsConfirmationPopupVisible = true;
    }

    public void ShowDurationEntry(TimeSpan currentDuration)
    {
        EntryPopupTitle = localizationManager["Title_AdjustDuration"];
        EntryPopupPlaceholder = localizationManager["Placeholder_EnterDurationMinutes"];

        EntryPopupDuration = currentDuration;

        if (EntryPopupDuration.TotalMinutes < AppConstants.ValidationLimits.MinWorkoutDurationMinutes)
            EntryPopupDuration = TimeSpan.FromMinutes(AppConstants.ValidationLimits.MinWorkoutDurationMinutes);

        IsEntryPopupVisible = true;
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

    [RelayCommand]
    private void CloseConfirmationPopup()
    {
        IsConfirmationPopupVisible = false;
        _pendingConfirmationAction = null;
    }

    [RelayCommand]
    private void SaveCorrectedDuration()
    {
        var minutes = EntryPopupDuration.TotalMinutes;

        switch (minutes)
        {
            case < AppConstants.ValidationLimits.MinWorkoutDurationMinutes:
                DurationValidationError?.Invoke("Error_DurationTooShort");
                return;
            case > AppConstants.ValidationLimits.MaxWorkoutDurationHours * AppConstants.Time.MinutesPerHour:
                DurationValidationError?.Invoke("Error_DurationTooLong");
                return;
        }

        IsEntryPopupVisible = false;
        DurationCorrectionSaved?.Invoke(EntryPopupDuration);
    }

    [RelayCommand]
    private void CloseEntryPopup()
    {
        IsEntryPopupVisible = false;
    }

    public void ShowRenamePopup(string currentName)
    {
        TempProgramName = currentName;
        IsRenamePopupVisible = true;
    }

    [RelayCommand]
    private void CloseRenamePopup()
    {
        IsRenamePopupVisible = false;
    }

    public void ShowCreatePopup()
    {
        CreateInputValue = string.Empty;
        IsCreatePopupVisible = true;
    }

    [RelayCommand]
    private void CloseCreatePopup()
    {
        IsCreatePopupVisible = false;
    }

    public void OpenCreateFoodPopup()
    {
        IsCreateFoodPopupVisible = true;
    }

    [RelayCommand]
    public void CloseCreateFoodPopup()
    {
        IsCreateFoodPopupVisible = false;
    }

    public event Action<string>? DurationValidationError;
    public event Action<TimeSpan>? DurationCorrectionSaved;
}