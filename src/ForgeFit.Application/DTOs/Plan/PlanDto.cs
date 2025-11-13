using ForgeFit.Application.DTOs.Goal;

namespace ForgeFit.Application.DTOs.Plan;

public record PlanDto(
    BodyGoalDto BodyGoal,
    NutritionGoalDto NutritionGoal,
    WorkoutGoalDto WorkoutGoal);