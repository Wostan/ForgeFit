namespace ForgeFit.Application.DTOs.Food;

public record FoodSearchDto(
    string ExternalId,
    string Label,
    string? BrandName,
    string Description);