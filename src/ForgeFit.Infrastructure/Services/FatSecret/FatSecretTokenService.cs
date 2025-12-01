using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Infrastructure.Configurations;
using ForgeFit.Infrastructure.Services.FatSecret.Models;
using Microsoft.Extensions.Options;

namespace ForgeFit.Infrastructure.Services.FatSecret;

public class FatSecretTokenService(
    HttpClient httpClient,
    IOptions<FoodApiSettings> settings) : IFatSecretTokenService
{
    private readonly FoodApiSettings _settings = settings.Value;

    private string _cachedToken = string.Empty;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_cachedToken) && _tokenExpiry > DateTime.UtcNow.AddMinutes(5)) return _cachedToken;

        var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.fatsecret.com/connect/token");

        var byteArray = Encoding.ASCII.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var values = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" }
        };
        request.Content = new FormUrlEncodedContent(values);

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"FatSecret Auth Failed: {response.StatusCode}, {errorContent}");
        }

        var tokenData = await response.Content.ReadFromJsonAsync<FatSecretTokenResponse>();

        if (tokenData is null || string.IsNullOrEmpty(tokenData.AccessToken))
            throw new Exception("Failed to retrieve FatSecret Access Token");

        _cachedToken = tokenData.AccessToken;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenData.ExpiresIn);

        return _cachedToken;
    }
}
