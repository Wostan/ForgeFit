namespace ForgeFit.Application.DTOs.Food;

public record CustomFoodDto(
    Guid Id,
    Guid? UserId,
    string? ExternalId,
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
    string ServingUnit,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
