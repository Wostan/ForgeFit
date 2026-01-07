using ForgeFit.MAUI.ViewModels;
using ForgeFit.MAUI.Views;
using ForgeFit.MAUI.Views.Auth;
using ForgeFit.MAUI.Views.Diary;

namespace ForgeFit.MAUI.Extensions;

public static class PresentationExtensions
{
    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<LoginPageViewModel>();

        builder.Services.AddTransient<DiaryPageViewModel>();
        builder.Services.AddTransient<MealDetailsPageViewModel>();
        builder.Services.AddTransient<FoodSearchPageViewModel>();

        return builder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<RegisterPageView>();
        builder.Services.AddTransient<LoginPageView>();

        builder.Services.AddTransient<DiaryPageView>();
        builder.Services.AddTransient<MealDetailsPageView>();
        builder.Services.AddTransient<FoodSearchPageView>();

        builder.Services.AddTransient<WorkoutPageView>();
        builder.Services.AddTransient<ProfilePageView>();

        return builder;
    }
}
