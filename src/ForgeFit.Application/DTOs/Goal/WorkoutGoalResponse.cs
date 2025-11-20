using ForgeFit.Domain.Enums.WorkoutEnums;

namespace ForgeFit.Application.DTOs.Goal;

public record WorkoutGoalResponse(
    Guid Id,
    int WorkoutsPerWeek,
    TimeSpan Duration,
    WorkoutType WorkoutType,
    DateTime CreatedAt);