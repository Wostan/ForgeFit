namespace ForgeFit.Application.DTOs.Food;

public record FoodItemDto(
    string ExternalId,
    string Label,
    int Calories,
    int Protein,
    int Fat,
    int Carbs,
    int Quantity
);