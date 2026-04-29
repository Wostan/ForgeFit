using CommunityToolkit.Mvvm.ComponentModel;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Food;

namespace ForgeFit.MAUI.ViewModels.Diary.Meals;

public partial class MealMacroStatsViewModel : ObservableObject
{
    [ObservableProperty] private double _caloriesProgress;
    [ObservableProperty] private double _carbsProgress;
    [ObservableProperty] private double _fatProgress;

    [ObservableProperty] private double _mealTargetCalories;

    [ObservableProperty] private string _mealTargetCaloriesDisplay = "-";
    [ObservableProperty] private double _mealTargetCarbs;
    [ObservableProperty] private string _mealTargetCarbsDisplay = "-";
    [ObservableProperty] private double _mealTargetFat;
    [ObservableProperty] private string _mealTargetFatDisplay = "-";
    [ObservableProperty] private double _mealTargetProtein;
    [ObservableProperty] private string _mealTargetProteinDisplay = "-";
    [ObservableProperty] private double _proteinProgress;
    [ObservableProperty] private double _totalCalories;
    [ObservableProperty] private double _totalCarbs;
    [ObservableProperty] private double _totalFat;
    [ObservableProperty] private double _totalProtein;
    [ObservableProperty] private double _totalFiber;
    [ObservableProperty] private double _totalSugar;
    [ObservableProperty] private double _totalSaturatedFat;
    [ObservableProperty] private double _totalSodium;

    public void CalculateTargets(double targetCalories)
    {
        MealTargetCalories = targetCalories;
        MealTargetCarbs = targetCalories * AppConstants.MacroRatios.Carbs / AppConstants.CaloriePerGram.Carbs;
        MealTargetProtein = targetCalories * AppConstants.MacroRatios.Protein / AppConstants.CaloriePerGram.Protein;
        MealTargetFat = targetCalories * AppConstants.MacroRatios.Fat / AppConstants.CaloriePerGram.Fat;

        MealTargetCaloriesDisplay = MealTargetCalories.ToString("F0");
        MealTargetCarbsDisplay = MealTargetCarbs.ToString("F0");
        MealTargetProteinDisplay = MealTargetProtein.ToString("F0");
        MealTargetFatDisplay = MealTargetFat.ToString("F0");
    }

    public void CalculateTotals(IEnumerable<FoodItemDto> items)
    {
        var foodItemDtos = items.ToList();

        TotalCalories = foodItemDtos.Sum(x => x.Calories);
        TotalCarbs = foodItemDtos.Sum(x => x.Carbs);
        TotalProtein = foodItemDtos.Sum(x => x.Protein);
        TotalFat = foodItemDtos.Sum(x => x.Fat);
        TotalFiber = foodItemDtos.Sum(x => x.Fiber);
        TotalSugar = foodItemDtos.Sum(x => x.Sugar);
        TotalSaturatedFat = foodItemDtos.Sum(x => x.SaturatedFat);
        TotalSodium = foodItemDtos.Sum(x => x.Sodium);

        CaloriesProgress = MealTargetCalories > 0 ? TotalCalories / MealTargetCalories : 0;
        CarbsProgress = MealTargetCarbs > 0 ? TotalCarbs / MealTargetCarbs : 0;
        ProteinProgress = MealTargetProtein > 0 ? TotalProtein / MealTargetProtein : 0;
        FatProgress = MealTargetFat > 0 ? TotalFat / MealTargetFat : 0;
    }

    public void Reset()
    {
        TotalCalories = 0;
        TotalCarbs = 0;
        TotalProtein = 0;
        TotalFat = 0;
        TotalFiber = 0;
        TotalSugar = 0;
        TotalSaturatedFat = 0;
        TotalSodium = 0;

        CaloriesProgress = 0;
        CarbsProgress = 0;
        ProteinProgress = 0;
        FatProgress = 0;

        MealTargetCalories = 0;
        MealTargetCarbs = 0;
        MealTargetProtein = 0;
        MealTargetFat = 0;

        MealTargetCaloriesDisplay = "-";
        MealTargetCarbsDisplay = "-";
        MealTargetProteinDisplay = "-";
        MealTargetFatDisplay = "-";
    }
}