using CommunityToolkit.Mvvm.ComponentModel;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.ViewModels.Diary.Tracking;

public partial class NutritionTrackingViewModel : Core.BaseViewModel
{
    private readonly IGoalService _goalService;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CaloriesProgress))]
    private double _currentCalories;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CarbsProgress))]
    private double _currentCarbs;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ProteinProgress))]
    private double _currentProtein;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(FatProgress))]
    private double _currentFat;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CaloriesProgress))]
    private double _targetCalories;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CarbsProgress))]
    private double _targetCarbs;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ProteinProgress))]
    private double _targetProtein;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(FatProgress))]
    private double _targetFat;

    [ObservableProperty] private string? _targetCaloriesDisplay;
    [ObservableProperty] private string? _targetCarbsDisplay;
    [ObservableProperty] private string? _targetProteinDisplay;
    [ObservableProperty] private string? _targetFatDisplay;

    public double CaloriesProgress => TargetCalories > 0 ? CurrentCalories / TargetCalories : 0;
    public double CarbsProgress => TargetCarbs > 0 ? CurrentCarbs / TargetCarbs : 0;
    public double ProteinProgress => TargetProtein > 0 ? CurrentProtein / TargetProtein : 0;
    public double FatProgress => TargetFat > 0 ? CurrentFat / TargetFat : 0;

    public NutritionTrackingViewModel(
        IGoalService goalService)
    {
        _goalService = goalService;
        SetLoadingState();
    }

    private void SetLoadingState()
    {
        TargetCaloriesDisplay = TargetCarbsDisplay = TargetProteinDisplay = TargetFatDisplay = "-";
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

                TargetCaloriesDisplay = TargetCalories.ToString("F0");
                TargetCarbsDisplay = TargetCarbs.ToString("F0");
                TargetProteinDisplay = TargetProtein.ToString("F0");
                TargetFatDisplay = TargetFat.ToString("F0");
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

        OnPropertyChanged(nameof(CaloriesProgress));
        OnPropertyChanged(nameof(CarbsProgress));
        OnPropertyChanged(nameof(ProteinProgress));
        OnPropertyChanged(nameof(FatProgress));
    }

    public void ResetNutrition()
    {
        CurrentCalories = 0;
        CurrentCarbs = 0;
        CurrentProtein = 0;
        CurrentFat = 0;

        OnPropertyChanged(nameof(CaloriesProgress));
        OnPropertyChanged(nameof(CarbsProgress));
        OnPropertyChanged(nameof(ProteinProgress));
        OnPropertyChanged(nameof(FatProgress));
    }
}