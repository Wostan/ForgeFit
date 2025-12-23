using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Views.Diary;

namespace ForgeFit.MAUI.ViewModels;

public partial class MealDetailsPageViewModel : ObservableObject
{
    public ObservableCollection<FoodItemDto> FoodItems { get; } = [];

    public MealDetailsPageViewModel()
    {
        LoadTestData();
    }

    [RelayCommand]
    private static async Task GoBack()
    {
        await Shell.Current.GoToAsync("..", false);
    }
    
    [RelayCommand]
    private static async Task GoToFoodSearch()
    {
        await Shell.Current.GoToAsync(nameof(FoodSearchPageView), false);
    }

    private void LoadTestData()
    {
        FoodItems.Add(new FoodItemDto("1", "Large Egg", 72, 0.4, 6.3, 4.8, "pcs", 1));
        FoodItems.Add(new FoodItemDto("2", "Oatmeal", 150, 27, 5, 2.5, "g", 40));
        FoodItems.Add(new FoodItemDto("3", "Banana", 105, 27, 1.3, 0.4, "pcs", 1));
        FoodItems.Add(new FoodItemDto("4", "Almonds", 164, 6, 6, 14, "g", 30));
    }
}
