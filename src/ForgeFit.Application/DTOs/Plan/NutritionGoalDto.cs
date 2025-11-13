namespace ForgeFit.Application.DTOs.Plan;

public record NutritionGoalDto(
    int Calories,
    int Carbs,
    int Protein,
    int Fat,
    int WaterGoalMl);