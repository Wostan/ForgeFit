namespace ForgeFit.MAUI.Models.DTOs.Food;

public record CustomFoodUpdateRequest(
    string Name,
    string? Brand,
    string? Barcode,
    double Calories,
    double Carbs,
    double Protein,
    double Fat,
    double Fiber,
    double Sugar,
    double SaturatedFat,
    double Sodium,
    double ServingSize,
    string ServingUnit
);
