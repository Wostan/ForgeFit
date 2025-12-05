using ForgeFit.MAUI.Handlers;
using ForgeFit.MAUI.Services;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Extensions;

public static class ServiceExtensions
{
    public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        // Handlers
        builder.Services.AddTransient<AuthHeaderHandler>();

        // Services
        builder.Services.AddScoped<IAlertService, AlertService>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        // API Client configuration
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
