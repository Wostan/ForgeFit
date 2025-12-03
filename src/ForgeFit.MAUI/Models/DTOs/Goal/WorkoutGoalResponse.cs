using ForgeFit.MAUI.Models.Enums.WorkoutEnums;

namespace ForgeFit.MAUI.Models.DTOs.Goal;

public record WorkoutGoalResponse(
    Guid Id,
    int WorkoutsPerWeek,
    TimeSpan Duration,
    WorkoutType WorkoutType);
