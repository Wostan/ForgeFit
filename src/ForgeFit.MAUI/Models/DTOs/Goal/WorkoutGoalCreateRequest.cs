using ForgeFit.MAUI.Models.Enums.WorkoutEnums;

namespace ForgeFit.MAUI.Models.DTOs.Goal;

public record WorkoutGoalCreateRequest(
    int WorkoutsPerWeek,
    TimeSpan Duration,
    WorkoutType WorkoutType);
