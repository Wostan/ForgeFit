using ForgeFit.Api.Middleware;

namespace ForgeFit.Api.Extensions;

public static class ExceptionMiddlewareExtension
{
    public static void UseExceptionHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}