namespace ForgeFit.Application.DTOs.Goal;

public record NutritionGoalDto(
    int Calories,
    int Carbs,
    int Protein,
    int Fat,
    int WaterGoalMl);