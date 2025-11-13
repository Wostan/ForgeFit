using ForgeFit.Domain.Enums.WorkoutEnums;

namespace ForgeFit.Application.DTOs.Plan;

public record WorkoutGoalDto(
    int WorkoutsPerWeek,
    TimeSpan Duration,
    WorkoutType WorkoutType);