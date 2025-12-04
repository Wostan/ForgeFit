using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.Views;

namespace ForgeFit.MAUI;

public partial class App : Application
{
    private readonly IAuthService _authService;

    public App(IAuthService authService)
    {
        InitializeComponent();
        Current!.UserAppTheme = AppTheme.Dark;
        
        _authService = authService;
    }

    protected override async void OnStart()
    {
        // try
        // {
        //     base.OnStart();
        //
        //     if (!await _authService.IsAuthenticatedAsync()) 
        //         await Shell.Current.GoToAsync(nameof(LoginPageView), false);
        // }
        // catch (Exception)
        // {
        //     await Shell.Current.GoToAsync(nameof(LoginPageView), false);
        // }
        
        await Shell.Current.GoToAsync($"///{nameof(DesignSystemPageView)}");
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
