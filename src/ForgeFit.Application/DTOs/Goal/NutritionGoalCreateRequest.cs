namespace ForgeFit.Application.DTOs.Goal;

public record NutritionGoalCreateRequest(
    int Calories,
    int Carbs,
    int Protein,
    int Fat,
    int WaterGoalMl);
