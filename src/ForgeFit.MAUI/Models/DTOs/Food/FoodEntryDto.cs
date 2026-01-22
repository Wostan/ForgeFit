using ForgeFit.MAUI.Models.Enums.FoodEnums;

namespace ForgeFit.MAUI.Models.DTOs.Food;

public record FoodEntryDto(
    Guid Id,
    DayTime DayTime,
    DateTime Date,
    List<FoodItemDto> FoodItems,
    double TotalCalories,
    double TotalCarbs,
    double TotalProtein,
    double TotalFat);
