namespace ForgeFit.Application.DTOs.Food;

public record FoodSearchResponse(
    string ExternalId,
    string Label,
    string? BrandName,
    double Calories,
    double Carbs,
    double Protein,
    double Fat,
    double Fiber,
    double Sugar,
    double SaturatedFat,
    double Sodium,
    string Serving
);
