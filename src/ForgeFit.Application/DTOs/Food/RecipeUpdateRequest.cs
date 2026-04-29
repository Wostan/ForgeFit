namespace ForgeFit.Application.DTOs.Food;

public record RecipeUpdateRequest(
    string Name,
    string? Description,
    List<FoodItemDto> Ingredients);
