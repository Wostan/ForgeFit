using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Registration;

public partial class GoalSettingsViewModel : BaseViewModel
{
    private readonly IBmiService _bmiService;
    private readonly IGoalRealismValidator _goalValidator;
    private readonly ILocalizationResourceManager _localizationManager;

    private double _currentHeight;
    private double _currentWeight;
    [ObservableProperty] private string _daysLeftText = string.Empty;

    [ObservableProperty]
    private DateTime _goalDueDate = DateTime.Today.AddMonths(AppConstants.GoalValidation.DefaultGoalMonthsAhead);

    [ObservableProperty] private bool _isDeadlineActive = true;
    [ObservableProperty] private bool _isNoDeadline;
    [ObservableProperty] private double _maxTargetWeight = AppConstants.ValidationLimits.MaxWeightKg;
    [ObservableProperty] private double _minTargetWeight = AppConstants.ValidationLimits.MinWeightKg;

    [ObservableProperty] private double _targetWeight;

    [ObservableProperty] private LocalizedString? _validationError;

    public GoalSettingsViewModel(
        IBmiService bmiService,
        IGoalRealismValidator goalValidator,
        ILocalizationResourceManager localizationManager)
    {
        _bmiService = bmiService;
        _goalValidator = goalValidator;
        _localizationManager = localizationManager;

        RecalculateDaysLeft();
    }

    public DateTime MinGoalDate => DateTime.Today.AddDays(AppConstants.GoalValidation.MinDaysToDeadline);

    public void SetCurrentMeasurements(double height, double weight)
    {
        _currentHeight = height;
        _currentWeight = weight;
        RecalculateTargetLimits();
        RecalculateDaysLeft();
    }

    partial void OnGoalDueDateChanged(DateTime value)
    {
        if (!IsNoDeadline) RecalculateDaysLeft();
    }

    [RelayCommand]
    private void ToggleDeadline()
    {
        IsNoDeadline = !IsNoDeadline;
    }

    partial void OnIsNoDeadlineChanged(bool value)
    {
        IsDeadlineActive = !value;
        RecalculateDaysLeft();

        if (ValidationError != null) ValidationError = null;
    }

    private void RecalculateDaysLeft()
    {
        if (IsNoDeadline)
        {
            DaysLeftText = string.Format(_localizationManager["Goal_DaysLeft"], "∞");
            return;
        }

        var days = (GoalDueDate - DateTime.Today).TotalDays;
        var intDays = (int)Math.Ceiling(days);

        DaysLeftText = string.Format(_localizationManager["Goal_DaysLeft"], intDays);
    }

    private void RecalculateTargetLimits()
    {
        if (_currentHeight <= 0) return;

        var heightM = _currentHeight / 100.0;
        var heightSq = heightM * heightM;

        MinTargetWeight = Math.Ceiling(AppConstants.BmiThresholds.UnderweightMax * heightSq);
        MaxTargetWeight = Math.Floor(AppConstants.BmiThresholds.OverweightMax * heightSq);

        if (TargetWeight < MinTargetWeight || TargetWeight > MaxTargetWeight || TargetWeight == 0)
        {
            if (_currentWeight >= MinTargetWeight && _currentWeight <= MaxTargetWeight)
                TargetWeight = _currentWeight;
            else
                TargetWeight = Math.Clamp(_currentWeight, MinTargetWeight, MaxTargetWeight);
        }
        else
        {
            OnPropertyChanged(nameof(TargetWeight));
        }
    }

    public bool ValidateStep()
    {
        if (TargetWeight <= 0)
        {
            ValidationError = new LocalizedString(() => _localizationManager["Error_InvalidWeight"]);
            return false;
        }

        var goalType = _bmiService.DetermineGoalType(_currentWeight, TargetWeight, _currentHeight);

        DateTime? goalDueDate = IsNoDeadline ? null : GoalDueDate;

        var validationResult = _goalValidator.ValidateGoalRealism(
            _currentWeight,
            TargetWeight,
            _currentHeight,
            goalDueDate,
            goalType,
            WeightUnit.Kg
        );

        if (validationResult.IsValid) return true;

        ValidationError = new LocalizedString(() => validationResult.ErrorMessage);
        return false;
    }

    public void ClearErrors()
    {
        ValidationError = null;
    }
}
