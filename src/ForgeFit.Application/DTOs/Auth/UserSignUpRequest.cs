using ForgeFit.Domain.Enums.ProfileEnums;

namespace ForgeFit.Application.DTOs.Auth;

public record UserSignUpRequest(
    string Email,
    string Password,
    string Username,
    string? Uri,
    DateTime DateOfBirth,
    Gender Gender,
    double Weight,
    string WeightUnit,
    double Height,
    string HeightUnit);