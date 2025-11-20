namespace ForgeFit.Application.DTOs.Goal;

public record NutritionGoalResponse(
    Guid Id,
    int Calories,
    int Carbs,
    int Protein,
    int Fat,
    int WaterGoalMl,
    DateTime CreatedAt);