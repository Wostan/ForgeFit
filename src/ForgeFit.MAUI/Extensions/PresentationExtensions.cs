using ForgeFit.MAUI.ViewModels;
using ForgeFit.MAUI.Views;
using ForgeFit.MAUI.Views.Diary;
using LoginPageView = ForgeFit.MAUI.Views.Auth.LoginPageView;
using RegisterPageView = ForgeFit.MAUI.Views.Auth.RegisterPageView;

namespace ForgeFit.MAUI.Extensions;

public static class PresentationExtensions
{
    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddScoped<LoginPageViewModel>();

        builder.Services.AddScoped<DiaryPageViewModel>();
        builder.Services.AddScoped<MealDetailsPageViewModel>();
        builder.Services.AddScoped<FoodSearchPageViewModel>();

        return builder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddScoped<RegisterPageView>();
        builder.Services.AddScoped<LoginPageView>();

        builder.Services.AddScoped<DiaryPageView>();
        builder.Services.AddScoped<MealDetailsPageView>();
        builder.Services.AddScoped<FoodSearchPageView>();

        builder.Services.AddScoped<WorkoutPageView>();

        builder.Services.AddScoped<ProfilePageView>();

        builder.Services.AddScoped<DesignSystemPageView>();

        return builder;
    }
}
