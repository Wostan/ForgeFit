using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Infrastructure.Configurations;
using ForgeFit.Infrastructure.Persistence;
using ForgeFit.Infrastructure.Repositories;
using ForgeFit.Infrastructure.Services;
using ForgeFit.Infrastructure.Services.ExerciseDBApi;
using ForgeFit.Infrastructure.Services.FatSecret;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ForgeFit.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly("ForgeFit.Infrastructure")
            ));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Configurations
        services.Configure<FoodApiSettings>(configuration.GetSection("FoodApiSettings"));
        services.Configure<ExerciseDbApiSettings>(configuration.GetSection("ExerciseDbApiSettings"));

        // Repos
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IBodyGoalRepository, BodyGoalRepository>();
        services.AddScoped<INutritionGoalRepository, NutritionGoalRepository>();
        services.AddScoped<IWorkoutGoalRepository, WorkoutGoalRepository>();
        services.AddScoped<IWorkoutProgramRepository, WorkoutProgramRepository>();
        services.AddScoped<IWorkoutEntryRepository, WorkoutEntryRepository>();
        services.AddScoped<IFoodEntryRepository, FoodEntryRepository>();
        services.AddScoped<IDrinkEntryRepository, DrinkEntryRepository>();

        // Http Clients
        services.AddHttpClient<IFatSecretTokenService, FatSecretTokenService>();
        services.AddHttpClient<IFoodApiService, FoodApiService>();
        services.AddHttpClient<IWorkoutApiService, WorkoutApiService>((provider, client) =>
        {
            var settings = provider.GetRequiredService<IOptions<ExerciseDbApiSettings>>().Value;
            client.BaseAddress = new Uri(settings.BaseUrl);
        });

        // Services
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
