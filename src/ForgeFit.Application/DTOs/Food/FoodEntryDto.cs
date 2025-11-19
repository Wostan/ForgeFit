namespace ForgeFit.Application.DTOs.Food;

public record FoodEntryDto(
    Guid Id,
    DateTime Date,
    List<FoodItemDto> FoodItems,
    double TotalCalories,
    double TotalProtein,
    double TotalCarbs,
    double TotalFat);