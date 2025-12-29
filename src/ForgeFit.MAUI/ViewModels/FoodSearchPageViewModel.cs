using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Food;
using ZXing.Net.Maui;

namespace ForgeFit.MAUI.ViewModels;

public partial class FoodSearchPageViewModel : ObservableObject
{
    [ObservableProperty] 
    private bool _isScannerVisible;
    
    [ObservableProperty]
    private bool _isTorchOn;
    
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    public BarcodeReaderOptions BarcodeOptions { get; } = new()
    {
        Formats = BarcodeFormats.All,
        AutoRotate = true,
        Multiple = false
    };
    
    public ObservableCollection<FoodSearchResponse> SearchResults { get; } = [];
    
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
}
