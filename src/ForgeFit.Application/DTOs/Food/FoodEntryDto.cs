using ForgeFit.Domain.Enums.FoodEnums;

namespace ForgeFit.Application.DTOs.Food;

public record FoodEntryDto(
    Guid Id,
    DayTime DayTime,
    DateTime Date,
    List<FoodItemDto> FoodItems,
    double TotalCalories,
    double TotalCarbs,
    double TotalProtein,
    double TotalFat);
