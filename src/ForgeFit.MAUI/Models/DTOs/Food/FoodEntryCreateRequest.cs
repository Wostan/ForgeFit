using ForgeFit.MAUI.Models.Enums.FoodEnums;

namespace ForgeFit.MAUI.Models.DTOs.Food;

public record FoodEntryCreateRequest(
    DayTime DayTime,
    DateTime Date,
    List<FoodItemDto> FoodItems);
