using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using ForgeFit.MAUI.Handlers;
using ForgeFit.MAUI.Services;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels;
using ForgeFit.MAUI.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Sharpnado.MaterialFrame;

namespace ForgeFit.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .UseSharpnadoMaterialFrame(false)
            .RegisterViewModels()
            .RegisterViews()
            .RegisterServices()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Montserrat-Regular.ttf", "MontserratRegular");
                fonts.AddFont("Montserrat-SemiBold.ttf", "MontserratSemiBold");
            })
            .ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android => android.OnCreate((activity, _) => MakeStatusBarTranslucent(activity)));

                static void MakeStatusBarTranslucent(Android.App.Activity activity)
                {
                    activity.Window!.SetFlags(Android.Views.WindowManagerFlags.LayoutNoLimits,
                        Android.Views.WindowManagerFlags.LayoutNoLimits);

                    activity.Window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentStatus);
                    activity.Window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentNavigation);

                    activity.Window.DecorView.SetBackgroundColor(Android.Graphics.Color.Black);

#pragma warning disable CA1422
                    activity.Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
                    activity.Window.SetNavigationBarColor(Android.Graphics.Color.Transparent);
#pragma warning restore CA1422
                }
#endif
            })
            .ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddHandler<Shell, CustomShellHandler>();
#endif
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddScoped<LoginPageViewModel>();

        return builder;
    }

    private static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddScoped<RegisterPageView>();
        builder.Services.AddScoped<LoginPageView>();
        builder.Services.AddScoped<DiaryPageView>();
        builder.Services.AddScoped<WorkoutPageView>();
        builder.Services.AddScoped<ProfilePageView>();
        builder.Services.AddScoped<DesignSystemPageView>();

        return builder;
    }

    private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<AuthHeaderHandler>();

        builder.Services.AddScoped<IAlertService, AlertService>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddHttpClient<IApiService, ApiService>(client =>
            {
                var baseAddress = DeviceInfo.Platform == DevicePlatform.Android
                    ? "http://10.0.2.2:5052"
                    : "http://localhost:5052";

                client.BaseAddress = new Uri(baseAddress);
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();

        return builder;
    }
}
