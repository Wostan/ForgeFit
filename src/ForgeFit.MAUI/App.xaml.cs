using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.Views.Auth;

namespace ForgeFit.MAUI;

public partial class App : Application
{
    private readonly IAuthService _authService;
    private readonly IServiceProvider _serviceProvider;

    public App(IAuthService authService, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        Current!.UserAppTheme = AppTheme.Dark;
        _authService = authService;
        _serviceProvider = serviceProvider;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window
        {
            Page = new ContentPage { BackgroundColor = Colors.Black }
        };

        NavigateUser(window);
        return window;
    }

    private async void NavigateUser(Window window)
    {
        try
        {
            if (await _authService.IsAuthenticatedAsync())
                window.Page = new AppShell();
            else
                window.Page = _serviceProvider.GetRequiredService<LoginPageView>();
        }
        catch
        {
            window.Page = _serviceProvider.GetRequiredService<LoginPageView>();
        }
    }
}
