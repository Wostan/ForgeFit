using ForgeFit.MAUI.Views.Diary;
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

        Routing.RegisterRoute(nameof(MealDetailsPageView), typeof(MealDetailsPageView));
        Routing.RegisterRoute(nameof(FoodSearchPageView), typeof(FoodSearchPageView));
    }
}
