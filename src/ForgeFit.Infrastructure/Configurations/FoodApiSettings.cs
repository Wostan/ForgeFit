namespace ForgeFit.Infrastructure.Configurations;

public class FoodApiSettings
{
    public string BaseUrl { get; set; } = "https://platform.fatsecret.com/rest/server.api/";
    public string RecognitionUrl { get; set; } = "https://platform.fatsecret.com/rest/image-recognition/v2";
    public string TokenUrl { get; set; } = "https://oauth.fatsecret.com/connect/token/";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Scope { get; set; } = "basic premier barcode localization";
    public string Region { get; set; } = "UA";
    public string Language { get; set; } = "uk";
}
