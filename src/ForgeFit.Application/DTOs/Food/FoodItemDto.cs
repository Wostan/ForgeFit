namespace ForgeFit.Application.DTOs.Food;

public record FoodItemDto(
    string ExternalId,
    string Label,
    double Calories,
    double Carbs,
    double Protein,
    double Fat,
    string ServingUnit,
    double Amount
);
