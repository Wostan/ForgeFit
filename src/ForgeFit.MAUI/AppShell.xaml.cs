using ForgeFit.MAUI.Views;

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
