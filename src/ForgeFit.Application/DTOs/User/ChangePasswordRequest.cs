namespace ForgeFit.Application.DTOs.User;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);