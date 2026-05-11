using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.User;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.Tracking;

public partial class WeightManagementViewModel : BaseViewModel
{
    private readonly IAlertService _alertService;
    private readonly IGoalService _goalService;
    private readonly ILocalizationResourceManager _localizationManager;
    private readonly IUserService _userService;

    [NotifyPropertyChangedFor(nameof(WeightProgress))]
    [NotifyPropertyChangedFor(nameof(WeightLeft))]
    [NotifyPropertyChangedFor(nameof(CurrentWeightInput))]
    [ObservableProperty]
    private double _currentWeight;

    [ObservableProperty] private string _currentWeightInput = string.Empty;
    private bool _suppressCurrentWeightInputChanged;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WeightProgress))]
    [NotifyPropertyChangedFor(nameof(WeightLeft))]
    private double _targetWeight;

    [ObservableProperty] private string? _targetWeightDisplay;
    private UserProfileDto? _userProfile;

    private CancellationTokenSource? _weightCts;

    public WeightManagementViewModel(
        IGoalService goalService,
        IUserService userService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager)
    {
        _goalService = goalService;
        _userService = userService;
        _alertService = alertService;
        _localizationManager = localizationManager;
        SetLoadingState();
    }

    public double WeightProgress => TargetWeight > 0 ? CurrentWeight / TargetWeight : 0;
    public double WeightLeft => Math.Abs(TargetWeight - CurrentWeight);

    private void SetLoadingState()
    {
        TargetWeightDisplay = "-";
    }

    public async Task LoadWeightDataAsync(CancellationToken token = default)
    {
        try
        {
            var bodyGoalTask = _goalService.GetBodyGoal(token);
            var profileTask = _userService.GetProfileAsync(token);

            await Task.WhenAll(bodyGoalTask, profileTask);
            if (token.IsCancellationRequested) return;

            if (bodyGoalTask.Result is { Success: true, Data: not null })
            {
                TargetWeight = bodyGoalTask.Result.Data.WeightGoal;
                TargetWeightDisplay = $"{TargetWeight:F1}";

                if (profileTask.Result is { Success: true, Data: not null })
                {
                    _userProfile = profileTask.Result.Data;
                    CurrentWeight = _userProfile.Weight;
                }
                else
                {
                    CurrentWeight = AppConstants.DefaultValues.DefaultWeightKg;
                }

                var weightText = CurrentWeight.ToString("F1");

                if (!string.Equals(CurrentWeightInput, weightText, StringComparison.Ordinal))
                {
                    _suppressCurrentWeightInputChanged = true;
                    try
                    {
                        CurrentWeightInput = weightText;
                    }
                    finally
                    {
                        _suppressCurrentWeightInputChanged = false;
                    }
                }
            }
        }
        catch (Exception)
        {
            if (!token.IsCancellationRequested) SetLoadingState();
        }
    }

    [RelayCommand]
    private void IncreaseWeight()
    {
        CurrentWeightInput = double.TryParse((string?)CurrentWeightInput, out var weight)
            ? (weight + AppConstants.DefaultValues.DefaultWeightChangeStep).ToString("F1")
            : TargetWeight.ToString("F1");
    }

    [RelayCommand]
    private void DecreaseWeight()
    {
        CurrentWeightInput = double.TryParse((string?)CurrentWeightInput, out var weight)
            ? Math.Max(0, weight - AppConstants.DefaultValues.DefaultWeightChangeStep).ToString("F1")
            : TargetWeight.ToString("F1");
    }

    private async Task SaveWeight(double weight)
    {
        _weightCts?.Cancel();
        _weightCts = new CancellationTokenSource();
        var token = _weightCts.Token;

        try
        {
            await Task.Delay(AppConstants.SearchConfig.DebounceDelayMs, token);
            if (token.IsCancellationRequested) return;

            if (_userProfile == null)
            {
                var loadResult = await _userService.GetProfileAsync(token);
                if (!loadResult.Success || loadResult.Data == null)
                    return;

                _userProfile = loadResult.Data;
            }

            var updatedProfile = _userProfile with { Weight = weight };
            var result = await _userService.UpdateProfileAsync(updatedProfile, token);

            if (!result.Success || result.Data == null)
            {
                var errorMsg = new LocalizedString(() => result.Message);
                await _alertService.ShowToastAsync(errorMsg.Localized);
                return;
            }

            _userProfile = updatedProfile;
            CurrentWeight = weight;

            _suppressCurrentWeightInputChanged = true;
            try
            {
                CurrentWeightInput = weight.ToString("F1");
            }
            finally
            {
                _suppressCurrentWeightInputChanged = false;
            }

            WeakReferenceMessenger.Default.Send(new WeightChangedMessage(nameof(WeightManagementViewModel)));
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
            if (token.IsCancellationRequested) return;
            var errorMsg = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]);
            await _alertService.ShowToastAsync(errorMsg.Localized);
        }
    }

    partial void OnCurrentWeightInputChanged(string value)
    {
        if (_suppressCurrentWeightInputChanged)
            return;

        if (!double.TryParse(value, out var weight))
            return;

        weight = Math.Round(weight, AppConstants.FormattingPrecision.WeightDecimalPlaces);

        if (Math.Abs(Math.Round(CurrentWeight, AppConstants.FormattingPrecision.WeightDecimalPlaces) - weight) <
            AppConstants.FormattingPrecision.DoubleComparisonEpsilon)
            return;

        CurrentWeight = weight;
        _ = SaveWeight(weight);
    }
}
