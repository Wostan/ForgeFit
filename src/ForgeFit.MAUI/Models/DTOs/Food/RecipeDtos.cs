namespace ForgeFit.MAUI.Models.DTOs.Food;

public record RecipeDto(
    Guid Id,
    Guid UserId,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<FoodItemDto> Ingredients,
    double TotalCalories,
    double TotalCarbs,
    double TotalProtein,
    double TotalFat,
    double TotalFiber,
    double TotalSugar,
    double TotalSaturatedFat,
    double TotalSodium
);

public record RecipeCreateRequest(
    string Name,
    string? Description,
    List<FoodItemDto> Ingredients
);

public record RecipeUpdateRequest(
    string Name,
    string? Description,
    List<FoodItemDto> Ingredients
);
