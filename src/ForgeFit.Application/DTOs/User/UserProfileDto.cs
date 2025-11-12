using ForgeFit.Domain.Enums.ProfileEnums;

namespace ForgeFit.Application.DTOs.User;

public record UserProfileDto(
    string Username,
    string? AvatarUrl,
    DateTime DateOfBirth,
    Gender Gender,
    double Weight,
    string WeightUnit,
    double Height,
    string HeightUnit);