namespace ForgeFit.Application.DTOs.Food;

public record FoodProductResponse(
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
    double Carbs,
    double Protein,
    double Fat
);
