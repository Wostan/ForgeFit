using CommunityToolkit.Mvvm.ComponentModel;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;

namespace ForgeFit.MAUI.ViewModels.Diary.Tracking;

public partial class NutritionTrackingViewModel : BaseViewModel
{
    private readonly IGoalService _goalService;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CaloriesProgress))]
    private double _currentCalories;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CarbsProgress))]
    private double _currentCarbs;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(FatProgress))]
    private double _currentFat;

    [ObservableProperty] private double _currentFiber;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ProteinProgress))]
    private double _currentProtein;

    [ObservableProperty] private double _currentSaturatedFat;
    [ObservableProperty] private double _currentSodium;
    [ObservableProperty] private double _currentSugar;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CaloriesProgress))]
    private double _targetCalories;

    [ObservableProperty] private string? _targetCaloriesDisplay;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CarbsProgress))]
    private double _targetCarbs;

    [ObservableProperty] private string? _targetCarbsDisplay;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(FatProgress))]
    private double _targetFat;

    [ObservableProperty] private string? _targetFatDisplay;

    [ObservableProperty] private double _targetFiber;

    [ObservableProperty] private string? _targetFiberDisplay;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ProteinProgress))]
    private double _targetProtein;

    [ObservableProperty] private string? _targetProteinDisplay;
    [ObservableProperty] private double _targetSaturatedFat;
    [ObservableProperty] private string? _targetSaturatedFatDisplay;
    [ObservableProperty] private double _targetSodium;
    [ObservableProperty] private string? _targetSodiumDisplay;
    [ObservableProperty] private double _targetSugar;
    [ObservableProperty] private string? _targetSugarDisplay;

    [ObservableProperty] private int _waterGoalMl;

    public NutritionTrackingViewModel(
        IGoalService goalService)
    {
        _goalService = goalService;
        SetLoadingState();
    }

    public double CaloriesProgress => TargetCalories > 0 ? CurrentCalories / TargetCalories : 0;
    public double CarbsProgress => TargetCarbs > 0 ? CurrentCarbs / TargetCarbs : 0;
    public double ProteinProgress => TargetProtein > 0 ? CurrentProtein / TargetProtein : 0;
    public double FatProgress => TargetFat > 0 ? CurrentFat / TargetFat : 0;
    public double FiberProgress => TargetFiber > 0 ? CurrentFiber / TargetFiber : 0;
    public double SugarProgress => TargetSugar > 0 ? CurrentSugar / TargetSugar : 0;
    public double SaturatedFatProgress => TargetSaturatedFat > 0 ? CurrentSaturatedFat / TargetSaturatedFat : 0;
    public double SodiumProgress => TargetSodium > 0 ? CurrentSodium / TargetSodium : 0;

    public bool IsSugarOverLimit => CurrentSugar > TargetSugar;
    public bool IsSaturatedFatOverLimit => CurrentSaturatedFat > TargetSaturatedFat;
    public bool IsSodiumOverLimit => CurrentSodium > TargetSodium;

    private void SetLoadingState()
    {
        TargetCaloriesDisplay = TargetCarbsDisplay = TargetProteinDisplay = TargetFatDisplay = "-";
        TargetFiberDisplay = TargetSugarDisplay = TargetSaturatedFatDisplay = TargetSodiumDisplay = "-";
    }

    private void CalculateMicronutrientTargets()
    {
        if (TargetCalories <= 0) return;

        TargetFiber = Math.Round(TargetCalories / 1000.0 * AppConstants.MicronutrientFactors.FiberGramsPer1000Kcal, 1);
        TargetSugar =
            Math.Round(
                TargetCalories * AppConstants.MicronutrientFactors.SugarCalorieRatio /
                AppConstants.CaloriePerGram.Carbs, 1);
        TargetSaturatedFat =
            Math.Round(
                TargetCalories * AppConstants.MicronutrientFactors.SaturatedFatCalorieRatio /
                AppConstants.CaloriePerGram.Fat, 1);
        TargetSodium = AppConstants.MicronutrientFactors.SodiumLimitMg;

        TargetFiberDisplay = TargetFiber.ToString("F1");
        TargetSugarDisplay = TargetSugar.ToString("F1");
        TargetSaturatedFatDisplay = TargetSaturatedFat.ToString("F1");
        TargetSodiumDisplay = TargetSodium.ToString("F0");

        OnPropertyChanged(nameof(FiberProgress));
        OnPropertyChanged(nameof(SugarProgress));
        OnPropertyChanged(nameof(SaturatedFatProgress));
        OnPropertyChanged(nameof(SodiumProgress));
        OnPropertyChanged(nameof(IsSugarOverLimit));
        OnPropertyChanged(nameof(IsSaturatedFatOverLimit));
        OnPropertyChanged(nameof(IsSodiumOverLimit));
    }

    public async Task LoadNutritionGoalsAsync(CancellationToken token = default)
    {
        try
        {
            var result = await _goalService.GetNutritionGoal(token);
            if (token.IsCancellationRequested) return;

            if (result is { Success: true, Data: not null })
            {
                var goal = result.Data;
                TargetCalories = goal.Calories;
                TargetCarbs = goal.Carbs;
                TargetProtein = goal.Protein;
                TargetFat = goal.Fat;
                WaterGoalMl = goal.WaterGoalMl;

                TargetCaloriesDisplay = TargetCalories.ToString("F0");
                TargetCarbsDisplay = TargetCarbs.ToString("F0");
                TargetProteinDisplay = TargetProtein.ToString("F0");
                TargetFatDisplay = TargetFat.ToString("F0");

                CalculateMicronutrientTargets();
            }
        }
        catch (Exception)
        {
            if (!token.IsCancellationRequested) SetLoadingState();
        }
    }

    public void UpdateCurrentNutrition(List<FoodEntryDto> entries)
    {
        CurrentCalories = entries.Sum(e => e.TotalCalories);
        CurrentCarbs = entries.Sum(e => e.TotalCarbs);
        CurrentProtein = entries.Sum(e => e.TotalProtein);
        CurrentFat = entries.Sum(e => e.TotalFat);
        CurrentFiber = entries.Sum(e => e.TotalFiber);
        CurrentSugar = entries.Sum(e => e.TotalSugar);
        CurrentSaturatedFat = entries.Sum(e => e.TotalSaturatedFat);
        CurrentSodium = entries.Sum(e => e.TotalSodium);

        OnPropertyChanged(nameof(CaloriesProgress));
        OnPropertyChanged(nameof(CarbsProgress));
        OnPropertyChanged(nameof(ProteinProgress));
        OnPropertyChanged(nameof(FatProgress));
        OnPropertyChanged(nameof(FiberProgress));
        OnPropertyChanged(nameof(SugarProgress));
        OnPropertyChanged(nameof(SaturatedFatProgress));
        OnPropertyChanged(nameof(SodiumProgress));
        OnPropertyChanged(nameof(IsSugarOverLimit));
        OnPropertyChanged(nameof(IsSaturatedFatOverLimit));
        OnPropertyChanged(nameof(IsSodiumOverLimit));
    }

    public void ResetNutrition()
    {
        CurrentCalories = 0;
        CurrentCarbs = 0;
        CurrentProtein = 0;
        CurrentFat = 0;
        CurrentFiber = 0;
        CurrentSugar = 0;
        CurrentSaturatedFat = 0;
        CurrentSodium = 0;

        OnPropertyChanged(nameof(CaloriesProgress));
        OnPropertyChanged(nameof(CarbsProgress));
        OnPropertyChanged(nameof(ProteinProgress));
        OnPropertyChanged(nameof(FatProgress));
        OnPropertyChanged(nameof(FiberProgress));
        OnPropertyChanged(nameof(SugarProgress));
        OnPropertyChanged(nameof(SaturatedFatProgress));
        OnPropertyChanged(nameof(SodiumProgress));
        OnPropertyChanged(nameof(IsSugarOverLimit));
        OnPropertyChanged(nameof(IsSaturatedFatOverLimit));
        OnPropertyChanged(nameof(IsSodiumOverLimit));
    }
}
