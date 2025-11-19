using FluentValidation;
using FluentValidation.AspNetCore;

namespace ForgeFit.Api.Extensions;

public static class ValidationExtension
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        
        return services;
    }
}