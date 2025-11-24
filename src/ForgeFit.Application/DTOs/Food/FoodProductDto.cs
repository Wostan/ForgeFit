namespace ForgeFit.Application.DTOs.Food;

public record FoodProductDto(
    string ExternalId,
    string Label,
    string? BrandName,
    List<FoodServingDto> Servings
);

public record FoodServingDto(
    string ServingId,
    double MetricAmount,
    string MetricUnit,
    double Calories,
    double Protein,
    double Fat,
    double Carbs
);