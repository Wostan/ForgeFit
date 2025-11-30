using ForgeFit.Domain.Enums.FoodEnums;

namespace ForgeFit.Application.DTOs.Food;

public record FoodEntryCreateRequest(
    DayTime DayTime,
    DateTime Date,
    List<FoodItemDto> FoodItems);