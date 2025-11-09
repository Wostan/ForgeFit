namespace ForgeFit.Application.DTOs.Auth;

public record UserSignUpResponse(
    string Id, 
    string Email,
    string Username, 
    string? AvatarUri);