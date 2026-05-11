using System.ComponentModel;
using CommunityToolkit.Maui.Core;
using ForgeFit.MAUI.ViewModels.Diary.PhotoRecognition;

namespace ForgeFit.MAUI.Views.Diary;

public partial class PhotoRecognitionPage : ContentPage
{
    private readonly PhotoRecognitionViewModel _vm;
    private double _originalBorderWidth;

    public PhotoRecognitionPage(PhotoRecognitionViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;

        vm.CaptureImageRequested = async ct => await FoodCamera.CaptureImage(ct);
        vm.PropertyChanged += Vm_PropertyChanged;
        vm.RetakeRequested = ResetCameraSize;
    }

    private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PhotoRecognitionViewModel.IsSuccessState) && _vm.IsSuccessState)
            AnimateCameraShrink();
    }

    private void AnimateCameraShrink()
    {
        if (CameraBorder.Width > 0 && _originalBorderWidth == 0)
            _originalBorderWidth = CameraBorder.Width;

        var anim = new Animation(v =>
        {
            CameraBorder.WidthRequest = v;
            CameraContainer.HeightRequest = v * 1.3333;
        }, CameraBorder.Width, 160);

        anim.Commit(this, "ShrinkAnim", length: 300, easing: Easing.CubicOut);
    }

    private void ResetCameraSize()
    {
        this.AbortAnimation("ShrinkAnim");

        var startWidth = CameraBorder.Width > 0 ? CameraBorder.Width : 160;
        var targetWidth = _originalBorderWidth > 0 ? _originalBorderWidth : startWidth;

        var anim = new Animation(v =>
        {
            CameraBorder.WidthRequest = v;
            CameraContainer.HeightRequest = v * 1.3333;
        }, startWidth, targetWidth);

        anim.Commit(this, "ExpandAnim", length: 300, easing: Easing.CubicOut,
            finished: (_, cancelled) =>
            {
                if (cancelled) return;
                CameraBorder.WidthRequest = -1;
                CameraBorder.HorizontalOptions = LayoutOptions.Fill;
                CameraContainer.HeightRequest = targetWidth * 1.3333;

                _vm.IsCameraActive = true;
            });
    }

    private void CameraContainer_SizeChanged(object? sender, EventArgs e)
    {
        if (!_vm.IsSuccessState && CameraContainer.Width > 0 && _originalBorderWidth == 0)
            CameraContainer.HeightRequest = CameraContainer.Width * 1.3333;
    }

    private async void FoodCamera_OnLoaded(object? sender, EventArgs e)
    {
        try
        {
            var cameras = await FoodCamera.GetAvailableCameras(CancellationToken.None);
            var rearCamera = cameras.FirstOrDefault(c => c.Position == CameraPosition.Rear);

            if (rearCamera is not null) FoodCamera.SelectedCamera = rearCamera;
        }
        catch
        {
        }
    }
}
