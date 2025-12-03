namespace ForgeFit.MAUI.Models.DTOs.Auth;

public record UserSignUpResponse(
    string AccessToken,
    string RefreshToken);
