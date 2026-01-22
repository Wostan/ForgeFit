using ForgeFit.MAUI.Models.DTOs.Goal;

namespace ForgeFit.MAUI.Models.DTOs.Plan;

public record PlanDto(
    BodyGoalResponse BodyGoal,
    NutritionGoalResponse NutritionGoal,
    WorkoutGoalResponse WorkoutGoal);
