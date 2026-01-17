using ForgeFit.MAUI.ViewModels;
using ZXing.Net.Maui;

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

        CameraView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.All,
            TryHarder = true,
            AutoRotate = true
        };

        IsVisible = false;
        Opacity = 0;

        CameraView.IsDetecting = false;

        CameraCurtain.Opacity = 1;
        CameraCurtain.IsVisible = true;
    }

    private static async void OnIsActiveChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not BarcodeScanner control) return;

        var isVisible = (bool)newValue;

        if (isVisible)
        {
            control.CameraView.IsEnabled = true;

            control.CameraCurtain.Opacity = 1;
            control.CameraCurtain.IsVisible = true;

            control.IsVisible = true;
            await control.FadeToAsync(1, 200, Easing.SinOut);

            MainThread.BeginInvokeOnMainThread(async void () =>
            {
                if (!control.IsActive) return;

                control.CameraView.IsDetecting = true;

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

            if (control.BindingContext is FoodSearchPageViewModel vm) vm.IsTorchOn = false;

            control.CameraView.IsDetecting = false;
            control.CameraView.IsTorchOn = false;

            control.CameraView.IsEnabled = false;

            await control.FadeToAsync(0, 200, Easing.SinIn);
            control.IsVisible = false;
        }
    }

    private void CameraView_OnBarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        var code = e.Results?.FirstOrDefault()?.Value;

        if (!string.IsNullOrEmpty(code))
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (BindingContext is FoodSearchPageViewModel vm) vm.BarcodeDetectedCommand.Execute(code);
            });
    }
}
