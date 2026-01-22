using ForgeFit.MAUI.Models.Enums.ProfileEnums;

namespace ForgeFit.MAUI.Models.DTOs.Workout;

public record PerformedSetDto(
    int Order,
    int Reps,
    double Weight,
    WeightUnit WeightUnit,
    bool IsCompleted
);
