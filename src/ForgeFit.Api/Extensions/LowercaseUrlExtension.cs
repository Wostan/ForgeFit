using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ForgeFit.Api.Extensions;

public static partial class LowercaseUrlExtension
{
    public static IServiceCollection AddLowercaseUrls(this IServiceCollection services)
    {
        services.AddRouting(options => 
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        services.AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
        });
        
        return services;
    }
    
    public partial class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            return value == null ? 
                null : 
                MyRegex().Replace(value.ToString()!, "$1-$2").ToLower();
        }

        [System.Text.RegularExpressions.GeneratedRegex("([a-z])([A-Z])")]
        private static partial System.Text.RegularExpressions.Regex MyRegex();
    }
}