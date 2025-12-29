using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Food;

namespace ForgeFit.MAUI.ViewModels;

public partial class FoodSearchPageViewModel : ObservableObject
{
    [ObservableProperty] 
    private bool _isScannerVisible;
    
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    private readonly List<FoodSearchResponse> _allFoodItems = [];

    public ObservableCollection<FoodSearchResponse> SearchResults { get; } = [];

    public FoodSearchPageViewModel()
    {
        LoadTestData();
    }
    
    [RelayCommand]
    private void ToggleScanner()
    {
        IsScannerVisible = !IsScannerVisible;
    }

    [RelayCommand]
    private void PerformSearch()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            SearchResults.Clear();
            foreach (var item in _allFoodItems) SearchResults.Add(item);
            return;
        }

        var filtered = _allFoodItems
            .Where(x => x.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                        (x.BrandName != null && x.BrandName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        SearchResults.Clear();
        foreach (var item in filtered) SearchResults.Add(item);
    }

    private void LoadTestData()
    {
        _allFoodItems.Clear();
        
        _allFoodItems.Add(new FoodSearchResponse("1", "Large Egg", null, 72, 0.4, 6.3, 4.8, "1 large"));
        _allFoodItems.Add(new FoodSearchResponse("2", "Banana", null, 105, 27, 1.3, 0.4, "1 medium"));
        _allFoodItems.Add(new FoodSearchResponse("3", "Chicken Breast (Raw)", null, 165, 0, 31, 3.6, "100 g"));
        _allFoodItems.Add(new FoodSearchResponse("4", "Avocado", null, 160, 8.5, 2, 14.7, "1/2 fruit"));
        _allFoodItems.Add(new FoodSearchResponse("5", "White Rice (Cooked)", null, 130, 28, 2.7, 0.3, "100 g"));

        _allFoodItems.Add(new FoodSearchResponse("6", "Творожные ленивые вареники", "Quaker", 150, 27, 5, 3, "40 g"));
        _allFoodItems.Add(new FoodSearchResponse("7", "Greek Yogurt 0%", "Chobaniiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii", 90, 6, 16, 0, "1 container"));
        _allFoodItems.Add(new FoodSearchResponse("8", "Whole Wheat Breaddddddddddddddddddddddddddddddddddddddddd", "Harry's", 80, 14, 4, 1, "1 slice"));
        _allFoodItems.Add(new FoodSearchResponse("9", "Peanut Butterrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr", "Jifffffffffffffffffffffffffffffffffffffff", 190, 8, 7, 16, "2 tbsp"));
        _allFoodItems.Add(new FoodSearchResponse("10", "Protein Bar", "Quest", 180, 22, 21, 7, "1 bar"));

        SearchResults.Clear();
        foreach (var item in _allFoodItems) SearchResults.Add(item);
    }
}
