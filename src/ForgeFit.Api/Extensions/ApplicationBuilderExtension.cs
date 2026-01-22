namespace ForgeFit.Api.Extensions;

public static class ApplicationBuilderExtension
{
    public static void UseApi(this IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
