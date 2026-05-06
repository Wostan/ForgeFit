using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.ViewModels.Core;

namespace ForgeFit.MAUI.ViewModels.Diary.MyProducts;

public partial class CreateCustomFoodViewModel(PopupManagerViewModel popupManager) : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string? _brand;
    [ObservableProperty] private string? _barcode;
    [ObservableProperty] private double _calories;
    [ObservableProperty] private double _carbs;
    [ObservableProperty] private double _protein;
    [ObservableProperty] private double _fat;
    [ObservableProperty] private double _fiber;
    [ObservableProperty] private double _sugar;
    [ObservableProperty] private double _saturatedFat;
    [ObservableProperty] private double _sodium;
    [ObservableProperty] private double _servingSize = 100;
    [ObservableProperty] private string _servingUnit = "g";

    public ObservableCollection<string> ServingUnits { get; } = ["g", "ml"];

    [RelayCommand]
    private void Close()
    {
        popupManager.CloseCreateFoodPopup();
    }

    [RelayCommand]
    private Task Save()
    {
        return Task.CompletedTask;
    }
}
