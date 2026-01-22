namespace ForgeFit.MAUI.Models.DTOs.Auth;

public record UserSignInResponse(
    string AccessToken,
    string RefreshToken);
