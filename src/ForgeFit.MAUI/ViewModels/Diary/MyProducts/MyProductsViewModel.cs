using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.ViewModels.Core;
using ForgeFit.MAUI.ViewModels.Diary.FoodSearch;

namespace ForgeFit.MAUI.ViewModels.Diary.MyProducts;

public partial class MyProductsViewModel(PopupManagerViewModel popupManager) : ObservableObject
{
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _isLoading;

    public ObservableCollection<FoodSearchItemViewModel> SearchResults { get; } = [];

    [RelayCommand]
    private void OpenCreateFoodPopup()
    {
        popupManager.OpenCreateFoodPopup();
    }
}
