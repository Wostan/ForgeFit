using ForgeFit.Domain.Enums.WorkoutEnums;

namespace ForgeFit.Application.DTOs.Goal;

public record WorkoutGoalDto(
    int WorkoutsPerWeek,
    TimeSpan Duration,
    WorkoutType WorkoutType);