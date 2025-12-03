namespace ForgeFit.MAUI.Models.DTOs.User;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);
