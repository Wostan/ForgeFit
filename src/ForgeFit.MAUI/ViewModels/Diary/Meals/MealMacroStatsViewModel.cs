using CommunityToolkit.Mvvm.ComponentModel;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Models.DTOs.Goal;
using ForgeFit.MAUI.Models.Enums.FoodEnums;

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

    public void CalculateTargets(NutritionGoalResponse dailyGoal, DayTime mealType)
    {
        var ratio = mealType switch
        {
            DayTime.Breakfast => 0.25,
            DayTime.Lunch => 0.35,
            DayTime.Dinner => 0.25,
            _ => 0.15
        };

        MealTargetCalories = dailyGoal.Calories * ratio;
        MealTargetCarbs = dailyGoal.Carbs * ratio;
        MealTargetProtein = dailyGoal.Protein * ratio;
        MealTargetFat = dailyGoal.Fat * ratio;

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