using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Food;
using ZXing.Net.Maui;

namespace ForgeFit.MAUI.ViewModels;

public partial class FoodSearchPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    
    [ObservableProperty] 
    private bool _isScannerVisible;
    
    [ObservableProperty]
    private bool _isTorchOn;
    
    
    [ObservableProperty]
    private bool _isFoodDetailsVisible;
    
    [ObservableProperty]
    private FoodProductResponse? _selectedFoodDetail;

    [ObservableProperty]
    private FoodServingDto? _selectedServing;

    [ObservableProperty]
    private double _inputAmount;
    
    [ObservableProperty] private double _currentCalories;
    [ObservableProperty] private double _currentProtein;
    [ObservableProperty] private double _currentFat;
    [ObservableProperty] private double _currentCarbs;
    
    public BarcodeReaderOptions BarcodeOptions { get; } = new()
    {
        Formats = BarcodeFormats.All,
        AutoRotate = true,
        Multiple = false
    };
    
    public ObservableCollection<FoodSearchResponse> SearchResults { get; } = [];
    
    public FoodSearchPageViewModel()
    {
        SearchResults.Add(new FoodSearchResponse("sdsd", "Testtest test", "Brandtest", 100, 10, 20, 5, ""));
    }
    
    [RelayCommand]
    private void ToggleScanner()
    {
        IsScannerVisible = !IsScannerVisible;
    }
    
    [RelayCommand]
    private void ToggleTorch()
    {
        IsTorchOn = !IsTorchOn;
    }
    
    [RelayCommand]
    private void CloseFoodDetails()
    {
        IsFoodDetailsVisible = false;
    }

    [RelayCommand]
    private void BarcodeDetected(BarcodeDetectionEventArgs e)
    {
        var barcode = e.Results?.FirstOrDefault()?.Value;
        if (string.IsNullOrEmpty(barcode)) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                HapticFeedback.Perform();
            }
            catch
            {
                // ignored
            }

            IsScannerVisible = false;
            PerformBarcodeSearch(barcode);
        });
    }

    private void PerformBarcodeSearch(string barcode)
    {
        // testing
        System.Diagnostics.Debug.WriteLine($"SEARCHING BARCODE: {barcode}");
        SearchText = barcode; 
    }

    [RelayCommand]
    private void PerformSearch()
    {
    }
    
    [RelayCommand]
    private async Task OpenFoodDetails(FoodSearchResponse? item)
    {
        System.Diagnostics.Debug.WriteLine($"COMMAND FIRED! Item: {item?.Label}");
        if (item is null) return;

        // var details = await _foodService.GetProductDetails(item.ExternalId);
        
        // mock
        var details = new FoodProductResponse(
            item.ExternalId,
            item.Label,
            item.BrandName,
            [
                new FoodServingDto("srv_1", 100, "g", item.Calories, item.Carbs, item.Protein, item.Fat),
                new FoodServingDto("srv_2", 1, "cup", item.Calories * 2.5, item.Carbs * 2.5, item.Protein * 2.5,
                    item.Fat * 2.5),
                new FoodServingDto("srv_3", 1, "oz", item.Calories * 0.28, item.Carbs * 0.28, item.Protein * 0.28, item.Fat * 0.28)
            ]
        );

        SelectedFoodDetail = details;
        
        var defaultServing = SelectedFoodDetail.Servings.FirstOrDefault();
        SelectedServing = defaultServing;

        InputAmount = defaultServing?.MetricAmount ?? 100;

        IsFoodDetailsVisible = true;
    }
    
    partial void OnInputAmountChanged(double value) => RecalculateMacros();
    partial void OnSelectedServingChanged(FoodServingDto? value)
    {
        if (value is null) return;
        
        InputAmount = value.MetricAmount;
        
        RecalculateMacros();
    }

    private void RecalculateMacros()
    {
        if (SelectedServing is null || SelectedServing.MetricAmount == 0) return;

        var k = InputAmount / SelectedServing.MetricAmount;

        CurrentCalories = SelectedServing.Calories * k;
        CurrentProtein = SelectedServing.Protein * k;
        CurrentFat = SelectedServing.Fat * k;
        CurrentCarbs = SelectedServing.Carbs * k;
    }
    
    [RelayCommand]
    private void AddFood()
    {
        System.Diagnostics.Debug.WriteLine($"Adding {InputAmount} of {SelectedServing?.MetricUnit}");
        
        IsFoodDetailsVisible = false;
    }
}
