namespace ForgeFit.Infrastructure.Configurations;

public class FoodApiSettings
{
    public string BaseUrl { get; set; } = "https://platform.fatsecret.com/rest/server.api/";
    public string TokenUrl { get; set; } = "https://oauth.fatsecret.com/connect/token/";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Scope { get; set; } = "basic";
    public string Region { get; set; } = "UA";
    public string Language { get; set; } = "ua";
}