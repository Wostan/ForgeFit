namespace ForgeFit.Application.DTOs.Auth;

public record UserSignUpResponse(
    string AccessToken,
    string RefreshToken);
