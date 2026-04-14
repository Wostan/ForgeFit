using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Goal;
using ForgeFit.MAUI.Models.DTOs.User;
using ForgeFit.MAUI.Models.Enums.GoalEnums;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Profile.Goals;

public partial class BodyGoalViewModel(
    IGoalService goalService,
    IBmiService bmiService,
    IGoalRealismValidator goalRealismValidator,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager)
    : BaseViewModel
{
    [ObservableProperty] private string _bodyGoalDescription = string.Empty;
    [ObservableProperty] private string _bodyGoalDueDate = string.Empty;

    [ObservableProperty] private string _bodyGoalEmoji = "🎯";
    [ObservableProperty] private string _bodyGoalTitle = string.Empty;
    [ObservableProperty] private string _bodyGoalType = string.Empty;
    [ObservableProperty] private string _bodyGoalWeight = string.Empty;
    private BodyGoalResponse? _currentBodyGoal;
    private UserProfileDto? _currentUserProfile;
    [ObservableProperty] private string? _editBodyGoalDescription;
    [ObservableProperty] private DateTime? _editBodyGoalDueDate;
    [ObservableProperty] private string? _editBodyGoalTitle;
    [ObservableProperty] private string? _editTargetWeight;

    [ObservableProperty] private bool _isEditBodyGoalPopupVisible;

    public void UpdateState(BodyGoalResponse goal)
    {
        _currentBodyGoal = goal;
        BodyGoalWeight = goal.WeightGoal.ToString("F1");
        BodyGoalTitle = goal.Title;
        BodyGoalDescription = goal.Description ?? string.Empty;
        BodyGoalDueDate = goal.DueDate.HasValue ? goal.DueDate.Value.ToString("dd.MM.yyyy") : "-";
        BodyGoalType = goal.GoalType switch
        {
            GoalType.MuscleGain => localizationManager["GoalType_MuscleGain"],
            GoalType.FatLoss => localizationManager["GoalType_FatLoss"],
            GoalType.WeightGain => localizationManager["GoalType_WeightGain"],
            _ => "Unknown"
        };

        BodyGoalEmoji = goal.GoalType switch
        {
            GoalType.MuscleGain => "💪",
            GoalType.FatLoss => "🔥",
            GoalType.WeightGain => "🥘",
            _ => "🎯"
        };
    }

    public void SetUserProfile(UserProfileDto profile)
    {
        _currentUserProfile = profile;
    }

    [RelayCommand]
    private void OpenEditBodyGoal()
    {
        if (_currentBodyGoal == null) return;

        EditTargetWeight = _currentBodyGoal.WeightGoal.ToString(CultureInfo.InvariantCulture);
        EditBodyGoalTitle = _currentBodyGoal.Title;
        EditBodyGoalDescription = _currentBodyGoal.Description;
        EditBodyGoalDueDate = _currentBodyGoal.DueDate;

        IsEditBodyGoalPopupVisible = true;
    }

    [RelayCommand]
    private void CloseEditBodyGoal()
    {
        IsEditBodyGoalPopupVisible = false;
    }

    [RelayCommand]
    private async Task SaveBodyGoal()
    {
        if (!double.TryParse(EditTargetWeight, CultureInfo.InvariantCulture, out var targetWeight))
        {
            await alertService.ShowToastAsync(localizationManager["Error_InvalidInput"]);
            return;
        }

        if (string.IsNullOrWhiteSpace(EditBodyGoalTitle))
        {
            await alertService.ShowToastAsync(localizationManager["Error_TitleRequired"]);
            return;
        }

        if (EditBodyGoalTitle.Length > AppConstants.ValidationLimits.MaxTitleLength)
        {
            await alertService.ShowToastAsync(localizationManager["Error_TitleTooLong"]);
            return;
        }

        if (!string.IsNullOrEmpty(EditBodyGoalDescription) && EditBodyGoalDescription.Length > AppConstants.ValidationLimits.MaxDescriptionLength)
        {
            await alertService.ShowToastAsync(localizationManager["Error_DescriptionTooLong"]);
            return;
        }

        var wUnit = _currentBodyGoal?.WeightUnit ?? WeightUnit.Kg;
        var isWeightValid = wUnit == WeightUnit.Kg
            ? targetWeight is >= AppConstants.ValidationLimits.MinWeightKg and <= AppConstants.ValidationLimits.MaxWeightKg
            : targetWeight is >= AppConstants.ValidationLimits.MinWeightLbs and <= AppConstants.ValidationLimits.MaxWeightLbs;

        if (!isWeightValid)
        {
            await alertService.ShowToastAsync(localizationManager["Error_InvalidWeightRange"]);
            return;
        }

        var userHeight = _currentUserProfile?.Height ?? 175;
        var currentWeight = _currentUserProfile?.Weight ?? targetWeight;

        var newGoalType = bmiService.DetermineGoalType(currentWeight, targetWeight, userHeight);

        var (isValid, errorMessage) = goalRealismValidator.ValidateGoalRealism(
            currentWeight,
            targetWeight,
            userHeight,
            EditBodyGoalDueDate,
            newGoalType,
            wUnit);

        if (!isValid)
        {
            await alertService.ShowToastAsync(errorMessage);
            return;
        }

        IsEditBodyGoalPopupVisible = false;
        IsLoading = true;

        try
        {
            var request = new BodyGoalCreateRequest(
                EditBodyGoalTitle,
                EditBodyGoalDescription,
                targetWeight,
                wUnit,
                EditBodyGoalDueDate,
                newGoalType
            );

            var result = await goalService.UpdateBodyGoal(request, CancellationToken.None);

            if (result is { Success: true, Data: not null })
            {
                UpdateState(result.Data);
                await alertService.ShowToastAsync(localizationManager["Success_GoalUpdated"]);
                OnGoalUpdated?.Invoke();
            }
            else
            {
                await alertService.ShowToastAsync(result.Message);
            }
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

    public event Action? OnGoalUpdated;
}