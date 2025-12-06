using ForgeFit.MAUI.ViewModels;
using ForgeFit.MAUI.Views;

namespace ForgeFit.MAUI.Extensions;

public static class PresentationExtensions
{
    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddScoped<LoginPageViewModel>();
        return builder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddScoped<RegisterPageView>();
        builder.Services.AddScoped<LoginPageView>();
        builder.Services.AddScoped<DiaryPageView>();
        builder.Services.AddScoped<WorkoutPageView>();
        builder.Services.AddScoped<ProfilePageView>();
        builder.Services.AddScoped<DesignSystemPageView>();

        return builder;
    }
}
