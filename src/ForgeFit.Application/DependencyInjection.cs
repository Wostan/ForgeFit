using System.Reflection;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Services;
using ForgeFit.Domain.Primitives.Interfaces;
using ForgeFit.Domain.Services;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeFit.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Domain Services
        services.AddScoped<IDailyNutritionPlanCalculationService, DailyNutritionPlanCalculationService>();
        services.AddScoped<IWorkoutPlanGenerationService, WorkoutPlanGenerationService>();
        services.AddScoped<IPlanGenerationService, PlanGenerationService>();
        
        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGoalService, GoalService>();
        services.AddScoped<IPlanService, PlanService>();
        
        // Mapper
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        
        return services;
    }
}