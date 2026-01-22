using ForgeFit.MAUI.Models.Enums.ProfileEnums;

namespace ForgeFit.MAUI.Models.DTOs.Workout;

public record WorkoutSetDto(
    Guid Id,
    int Order,
    int Reps,
    TimeSpan RestTime,
    double Weight,
    WeightUnit WeightUnit);
