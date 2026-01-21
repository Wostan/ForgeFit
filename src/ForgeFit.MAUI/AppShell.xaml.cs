using ForgeFit.MAUI.Views.Auth;
using ForgeFit.MAUI.Views.Diary;
using ForgeFit.MAUI.Views.Workout;
using LoginPageView = ForgeFit.MAUI.Views.Auth.LoginPageView;

namespace ForgeFit.MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(LoginPageView), typeof(LoginPageView));
        Routing.RegisterRoute(nameof(RegistrationPageView), typeof(RegistrationPageView));

        Routing.RegisterRoute(nameof(MealDetailsPageView), typeof(MealDetailsPageView));
        Routing.RegisterRoute(nameof(FoodSearchPageView), typeof(FoodSearchPageView));
        
        Routing.RegisterRoute(nameof(ActiveWorkoutPageView), typeof(ActiveWorkoutPageView));
        Routing.RegisterRoute(nameof(ExerciseSearchPageView), typeof(ExerciseSearchPageView));
    }
}
