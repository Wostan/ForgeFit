using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeFit.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMapster();
        
        return services;
    }
}