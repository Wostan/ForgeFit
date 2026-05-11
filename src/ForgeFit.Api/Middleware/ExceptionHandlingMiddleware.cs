using System.Text.Json;
using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Exceptions.AuthExceptions;
using ForgeFit.Domain.Exceptions;

namespace ForgeFit.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = exception switch
        {
            DomainValidationException or BadRequestException or UriFormatException => StatusCodes.Status400BadRequest,
            InvalidCredentialsException => StatusCodes.Status401Unauthorized,
            UnauthorizedAccessException => StatusCodes.Status403Forbidden,
            NotFoundException => StatusCodes.Status404NotFound,
            EmailAlreadyExistsException => StatusCodes.Status409Conflict,
            ServiceUnavailableException => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new { message = exception.Message });

        return context.Response.WriteAsync(result);
    }
}
