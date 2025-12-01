namespace ForgeFit.Application.DTOs.Auth;

public record UserSignInResponse(
    string AccessToken,
    string RefreshToken);
