namespace ForgeFit.Application.DTOs.Auth;

public record UserSignInResponse(
    string Id,
    string Email, 
    string AccessToken,
    string RefreshToken);