using ForgeFit.Application.DTOs.Goal;

namespace ForgeFit.Application.DTOs.Plan;

public record PlanDto(
    BodyGoalResponse BodyGoal,
    NutritionGoalResponse NutritionGoal,
    WorkoutGoalResponse WorkoutGoal);
