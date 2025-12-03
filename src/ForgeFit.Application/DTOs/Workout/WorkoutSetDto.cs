using ForgeFit.Domain.Enums.ProfileEnums;

namespace ForgeFit.Application.DTOs.Workout;

public record WorkoutSetDto(
    Guid Id,
    int Order,
    int Reps,
    TimeSpan RestTime,
    double Weight,
    WeightUnit WeightUnit);
