using ForgeFit.Application;
using ForgeFit.Application.Common.Interfaces;
using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Infrastructure.Persistence;
using ForgeFit.Infrastructure.Repositories;
using ForgeFit.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace ForgeFit.Api.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        
        return services;
    }
    
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly("ForgeFit.Infrastructure")
            ));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
    }
    
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddApplicationServices();
        return services;
    }
}