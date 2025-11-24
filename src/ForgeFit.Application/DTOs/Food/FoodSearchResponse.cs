namespace ForgeFit.Application.DTOs.Food;

public record FoodSearchResponse(
    string ExternalId,
    string Label,
    string? BrandName,
    string Description);