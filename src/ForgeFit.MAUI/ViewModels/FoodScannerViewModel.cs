using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ForgeFit.MAUI.ViewModels;

public partial class FoodScannerViewModel : ObservableObject
{
    [ObservableProperty] private bool _isScannerVisible;
    [ObservableProperty] private bool _isTorchOn;

    private bool _isProcessingBarcode;

    public Func<string, Task>? BarcodeDetectedCallback { get; set; }

    [RelayCommand]
    private void ToggleScanner()
    {
        IsScannerVisible = !IsScannerVisible;
        if (IsScannerVisible)
            _isProcessingBarcode = false;
    }

    [RelayCommand]
    private void ToggleTorch()
    {
        IsTorchOn = !IsTorchOn;
    }

    [RelayCommand]
    private async Task BarcodeDetected(string barcode)
    {
        if (string.IsNullOrEmpty(barcode) || BarcodeDetectedCallback == null || _isProcessingBarcode) return;

        _isProcessingBarcode = true;

        try
        {
            HapticFeedback.Perform();
        }
        catch
        {
            // ignored
        }

        await BarcodeDetectedCallback(barcode);
    }

    public void ResetState()
    {
        IsScannerVisible = false;
        IsTorchOn = false;
        _isProcessingBarcode = false;
    }
}
