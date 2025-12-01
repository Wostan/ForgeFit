using ForgeFit.Domain.Enums.ProfileEnums;

namespace ForgeFit.Application.DTOs.Workout;

public record PerformedSetDto(
    int Order,
    int Reps,
    double Weight,
    WeightUnit WeightUnit,
    bool IsCompleted
);
