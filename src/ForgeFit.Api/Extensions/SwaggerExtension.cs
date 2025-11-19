using Microsoft.OpenApi.Models;

namespace ForgeFit.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ForgeFit API",
                Version = "v1"
            });
        });

        return services;
    }
}