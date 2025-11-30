using ForgeFit.Domain.Enums.WorkoutEnums;

namespace ForgeFit.Application.DTOs.Goal;

public record WorkoutGoalCreateRequest(
    int WorkoutsPerWeek,
    TimeSpan Duration,
    WorkoutType WorkoutType);