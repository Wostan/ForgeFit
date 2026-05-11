namespace ForgeFit.MAUI.Views.Auth.Templates;

public partial class Step1CredentialsView : ContentView
{
    private const double Distance = 8.0;
    private const uint Duration = 4000;

    private bool _isBouncing;

    public Step1CredentialsView()
    {
        InitializeComponent();

        Loaded += (_, _) => StartBounceAnimation();
        Unloaded += (_, _) => StopBounceAnimation();
    }

    private async void StartBounceAnimation()
    {
        if (_isBouncing || ForgeFitLabel == null) return;

        _isBouncing = true;

        while (_isBouncing)
            try
            {
                await ForgeFitLabel.TranslateToAsync(0, Distance, Duration / 2, Easing.SinInOut);

                if (!_isBouncing) break;

                await ForgeFitLabel.TranslateToAsync(0, 0, Duration / 2, Easing.SinInOut);
            }
            catch (Exception)
            {
                break;
            }
    }

    private void StopBounceAnimation()
    {
        _isBouncing = false;

        ForgeFitLabel?.CancelAnimations();
        ForgeFitLabel?.TranslationY = 0;
    }
}
