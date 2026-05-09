using BarcodeScanner.Mobile;
using AddFoodPageViewModel = ForgeFit.MAUI.ViewModels.Diary.AddFood.AddFoodPageViewModel;

namespace ForgeFit.MAUI.Views.Controls.Diary;

public partial class BarcodeScanner : ContentView
{
    public static readonly BindableProperty IsActiveProperty = BindableProperty.Create(
        nameof(IsActive),
        typeof(bool),
        typeof(BarcodeScanner),
        false,
        propertyChanged: OnIsActiveChanged);

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public BarcodeScanner()
    {
        InitializeComponent();

        Methods.SetSupportBarcodeFormat(BarcodeFormats.All);

        IsVisible = false;
        Opacity = 0;

        CameraView.IsScanning = false;

        CameraCurtain.Opacity = 1;
        CameraCurtain.IsVisible = true;
    }

    private static async void OnIsActiveChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not BarcodeScanner control) return;

        var isVisible = (bool)newValue;

        if (isVisible)
        {
            control.CameraView.IsScanning = true;

            control.CameraCurtain.Opacity = 1;
            control.CameraCurtain.IsVisible = true;

            control.IsVisible = true;
            await control.FadeToAsync(1, 200, Easing.SinOut);

            MainThread.BeginInvokeOnMainThread(async void () =>
            {
                if (!control.IsActive) return;

                await Task.Delay(200);

                if (!control.IsActive) return;

                await control.CameraCurtain.FadeToAsync(0, 200, Easing.SinIn);
                control.CameraCurtain.IsVisible = false;
            });
        }
        else
        {
            control.CameraCurtain.IsVisible = true;
            await control.CameraCurtain.FadeToAsync(1, 200, Easing.SinOut);

            if (control.BindingContext is AddFoodPageViewModel vm)
                vm.ScannerVM.IsTorchOn = false;

            control.CameraView.IsScanning = false;
            control.CameraView.TorchOn = false;

            await control.FadeToAsync(0, 200, Easing.SinIn);
            control.IsVisible = false;
        }
    }

    private void CameraView_OnDetected(object? sender, OnDetectedEventArg e)
    {
        var code = e.BarcodeResults?.FirstOrDefault()?.DisplayValue;

        if (!string.IsNullOrEmpty(code))
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (BindingContext is AddFoodPageViewModel vm)
                    vm.ScannerVM.BarcodeDetectedCommand.Execute(code);
            });
    }
}