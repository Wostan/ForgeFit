namespace ForgeFit.MAUI.Models.DTOs.Food;

public record FoodSearchResponse(
    string ExternalId,
    string Label,
    string? BrandName,
    double Calories,
    double Carbs,
    double Protein,
    double Fat,
    string ServingUnit
);
