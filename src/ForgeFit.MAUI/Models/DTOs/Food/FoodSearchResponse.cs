namespace ForgeFit.MAUI.Models.DTOs.Food;

public record FoodSearchResponse(
    string ExternalId,
    string Label,
    string? BrandName,
    string Description);
