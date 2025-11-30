using System.Text.Json.Serialization;

namespace ForgeFit.Api.Extensions;

public static class EnumConverterExtension
{
    public static IServiceCollection AddEnumConverter(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        return services;
    }
}