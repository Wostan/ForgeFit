namespace ForgeFit.Application.DTOs.Food;

public record RecipeCreateRequest(
    string Name,
    string? Description,
    List<FoodItemDto> Ingredients);
