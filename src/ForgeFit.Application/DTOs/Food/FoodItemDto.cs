namespace ForgeFit.Application.DTOs.Food;

public record FoodItemDto(
    string ExternalId,
    string Label,
    double Calories,
    double Carbs,
    double Protein,
    double Fat,
    double Fiber,
    double Sugar,
    double SaturatedFat,
    double Sodium,
    string ServingUnit,
    double Amount
);
