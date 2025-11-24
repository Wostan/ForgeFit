namespace ForgeFit.Application.DTOs.Food;

public record FoodItemDto(
    string ExternalId,
    string Label,
    double Calories, 
    double Protein,
    double Fat,
    double Carbs,
    string ServingUnit,
    double Amount
);