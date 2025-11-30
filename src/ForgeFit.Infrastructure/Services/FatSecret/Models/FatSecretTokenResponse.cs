using System.Text.Json.Serialization;

namespace ForgeFit.Infrastructure.Services.FatSecret.Models;

public record FatSecretTokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("token_type")] string TokenType
);