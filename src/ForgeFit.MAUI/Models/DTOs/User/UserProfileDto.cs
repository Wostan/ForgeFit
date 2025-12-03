using ForgeFit.MAUI.Models.Enums.ProfileEnums;

namespace ForgeFit.MAUI.Models.DTOs.User;

public record UserProfileDto(
    string Username,
    string? AvatarUrl,
    DateTime DateOfBirth,
    Gender Gender,
    double Weight,
    WeightUnit WeightUnit,
    double Height,
    HeightUnit HeightUnit);
