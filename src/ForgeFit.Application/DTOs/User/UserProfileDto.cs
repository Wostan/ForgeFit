using ForgeFit.Domain.Enums.ProfileEnums;

namespace ForgeFit.Application.DTOs.User;

public record UserProfileDto(
    string Username,
    string? AvatarUrl,
    DateTime DateOfBirth,
    Gender Gender,
    double Weight,
    WeightUnit WeightUnit,
    double Height,
    HeightUnit HeightUnit);