using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Models.Enums.FoodEnums;
using ForgeFit.MAUI.ViewModels.Core;
using ForgeFit.MAUI.Views.Diary;

namespace ForgeFit.MAUI.ViewModels.Diary.Meals;

public partial class MealDashboardViewModel : BaseViewModel
{
    private DateTime _selectedDate = DateTime.Today;
    public MealDashboardItem Breakfast { get; } = new(DayTime.Breakfast);
    public MealDashboardItem Lunch { get; } = new(DayTime.Lunch);
    public MealDashboardItem Dinner { get; } = new(DayTime.Dinner);
    public MealDashboardItem Snack { get; } = new(DayTime.Snack);

    public void SetSelectedDate(DateTime selectedDate)
    {
        _selectedDate = selectedDate;
    }

    public void UpdateMealItems(List<FoodEntryDto> entries, double targetCalories)
    {
        UpdateMealItem(Breakfast, entries, targetCalories);
        UpdateMealItem(Lunch, entries, targetCalories);
        UpdateMealItem(Dinner, entries, targetCalories);
        UpdateMealItem(Snack, entries, targetCalories);
    }

    private void UpdateMealItem(MealDashboardItem item, List<FoodEntryDto> entries, double targetCalories)
    {
        var mealEntries = entries.Where(e => e.DayTime == item.Type).ToList();

        if (mealEntries.Count != 0)
        {
            item.CurrentCalories = mealEntries.Sum(e => e.TotalCalories);
            item.EntryId = mealEntries.FirstOrDefault()?.Id;
            item.HasEntry = true;
        }
        else
        {
            item.CurrentCalories = 0;
            item.EntryId = null;
            item.HasEntry = false;
        }

        var ratio = item.Type switch
        {
            DayTime.Breakfast => AppConstants.MealRatios.Breakfast,
            DayTime.Lunch => AppConstants.MealRatios.Lunch,
            DayTime.Dinner => AppConstants.MealRatios.Dinner,
            _ => AppConstants.MealRatios.Snack
        };

        item.TargetCalories = targetCalories * ratio;
    }

    public void ResetMeals()
    {
        UpdateMealItem(Breakfast, [], 0);
        UpdateMealItem(Lunch, [], 0);
        UpdateMealItem(Dinner, [], 0);
        UpdateMealItem(Snack, [], 0);
    }

    [RelayCommand]
    private async Task GoToMealDetails(DayTime mealType)
    {
        var item = mealType switch
        {
            DayTime.Breakfast => Breakfast,
            DayTime.Lunch => Lunch,
            DayTime.Dinner => Dinner,
            _ => Snack
        };

        var route = $"{nameof(MealDetailsPageView)}?" +
                    $"Date={_selectedDate:yyyy-MM-dd}&" +
                    $"MealType={mealType}&" +
                    $"TargetCalories={item.TargetCalories.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

        if (item.EntryId != null) 
            route += $"&EntryId={item.EntryId}";
        
        await Shell.Current.GoToAsync(route);
    }
}

public partial class MealDashboardItem(DayTime type) : ObservableObject
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(Progress))]
    private double _currentCalories;

    [ObservableProperty] private Guid? _entryId;

    [ObservableProperty] private bool _hasEntry;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(Progress))]
    private double _targetCalories;

    public DayTime Type { get; } = type;

    public double Progress => TargetCalories > 0 ? CurrentCalories / TargetCalories : 0;
}