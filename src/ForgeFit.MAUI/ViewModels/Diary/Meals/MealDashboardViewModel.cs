using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Models.Enums.FoodEnums;
using ForgeFit.MAUI.Views.Diary;

namespace ForgeFit.MAUI.ViewModels.Diary.Meals;

public partial class MealDashboardViewModel : Core.BaseViewModel
{
    public MealDashboardItem Breakfast { get; } = new(DayTime.Breakfast);
    public MealDashboardItem Lunch { get; } = new(DayTime.Lunch);
    public MealDashboardItem Dinner { get; } = new(DayTime.Dinner);
    public MealDashboardItem Snack { get; } = new(DayTime.Snack);

    private DateTime _selectedDate = DateTime.Today;

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
            DayTime.Breakfast => 0.25,
            DayTime.Lunch => 0.35,
            DayTime.Dinner => 0.25,
            _ => 0.15
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

        var route = $"{nameof(MealDetailsPageView)}?Date={_selectedDate:yyyy-MM-dd}&MealType={mealType}";

        if (item.EntryId != null) route += $"&EntryId={item.EntryId}";

        await Shell.Current.GoToAsync(route);
    }
}

public partial class MealDashboardItem(DayTime type) : ObservableObject
{
    public DayTime Type { get; } = type;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(Progress))]
    private double _currentCalories;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(Progress))]
    private double _targetCalories;

    [ObservableProperty] private Guid? _entryId;

    [ObservableProperty] private bool _hasEntry;

    public double Progress => TargetCalories > 0 ? CurrentCalories / TargetCalories : 0;
}