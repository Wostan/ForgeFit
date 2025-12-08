using ForgeFit.MAUI.ViewModels;

namespace ForgeFit.MAUI.Views;

public partial class LoginPageView : ContentPage
{
    private const double Distance = 8.0;
    private const uint Duration = 4000;

    private bool _isBouncing;

    public LoginPageView(LoginPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartBounceAnimation();
    }

    protected override void OnDisappearing()
    {
        StopBounceAnimation();
        base.OnDisappearing();
    }

    private async void StartBounceAnimation()
    {
        if (_isBouncing) return;

        _isBouncing = true;
        while (_isBouncing)
        {
            await ForgeFitLabel.TranslateToAsync(0, Distance, Duration / 2, Easing.SinInOut);
            if (!_isBouncing) break;
            await ForgeFitLabel.TranslateToAsync(0, 0, Duration / 2, Easing.SinInOut);
        }
    }

    private void StopBounceAnimation()
    {
        _isBouncing = false;
        ForgeFitLabel.TranslationY = 0;
    }
}
