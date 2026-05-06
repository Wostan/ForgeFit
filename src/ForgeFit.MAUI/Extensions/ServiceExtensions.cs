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
        builder.Services.AddTransient<RefreshTokenHandler>();

        // Services
        builder.Services.AddScoped<IAlertService, AlertService>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddScoped<IDiaryService, DiaryService>();
        builder.Services.AddScoped<IFoodService, FoodService>();
        builder.Services.AddScoped<ICustomFoodService, CustomFoodService>();
        builder.Services.AddScoped<IDrinkTrackingService, DrinkTrackingService>();

        builder.Services.AddScoped<IWorkoutTrackingService, WorkoutTrackingService>();
        builder.Services.AddScoped<IWorkoutExerciseService, WorkoutExerciseService>();
        builder.Services.AddScoped<IWorkoutProgramService, WorkoutProgramService>();

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IGoalService, GoalService>();
        builder.Services.AddScoped<IPlanService, PlanService>();

        builder.Services.AddSingleton<IBmiService, BmiService>();
        builder.Services.AddSingleton<IGoalRealismValidator, GoalRealismValidator>();

        // API Client configuration
        var baseAddress = DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:8080"
            : "http://localhost:8080";

        builder.Services.AddHttpClient<IApiService, ApiService>(client =>
            {
                client.BaseAddress = new Uri(baseAddress);
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .AddHttpMessageHandler<RefreshTokenHandler>();

        builder.Services.AddHttpClient("RefreshClient", client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        return builder;
    }
}
