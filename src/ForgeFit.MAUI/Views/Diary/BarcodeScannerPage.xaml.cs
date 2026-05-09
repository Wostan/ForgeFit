using BarcodeScanner.Mobile;
using ForgeFit.MAUI.ViewModels.Diary.FoodSearch;

namespace ForgeFit.MAUI.Views.Diary;

public partial class BarcodeScannerPage : ContentPage
{
    public BarcodeScannerPage(FoodScannerViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CameraView.IsScanning = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        CameraView.IsScanning = false;
        if (BindingContext is FoodScannerViewModel vm)
        {
            vm.IsTorchOn = false;
        }
        CameraView.TorchOn = false;
    }

    private void CameraView_OnDetected(object? sender, OnDetectedEventArg e)
    {
        var code = e.BarcodeResults?.FirstOrDefault()?.DisplayValue;
        if (!string.IsNullOrEmpty(code) && BindingContext is FoodScannerViewModel vm)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                vm.BarcodeDetectedCommand.Execute(code);
            });
        }
    }
}
