using ForgeFit.MAUI.Views;
using LoginPageView = ForgeFit.MAUI.Views.Auth.LoginPageView;
using RegisterPageView = ForgeFit.MAUI.Views.Auth.RegisterPageView;

namespace ForgeFit.MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(LoginPageView), typeof(LoginPageView));
        Routing.RegisterRoute(nameof(RegisterPageView), typeof(RegisterPageView));
    }
}
