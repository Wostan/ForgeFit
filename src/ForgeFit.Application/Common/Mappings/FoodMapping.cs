using Mapster;
using ForgeFit.Application.DTOs.Food;
using ForgeFit.Domain.Aggregates.FoodAggregate;
using ForgeFit.Domain.ValueObjects.FoodValueObjects;

namespace ForgeFit.Application.Common.Mappings;

public class FoodMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<FoodEntry, FoodEntryDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.DayTime, src => src.DayTime)
            .Map(dest => dest.Date, src => src.Date)
            .Map(dest => dest.TotalCalories, src => src.Calories)
            .Map(dest => dest.TotalProtein, src => src.Protein)
            .Map(dest => dest.TotalFat, src => src.Fat)
            .Map(dest => dest.TotalCarbs, src => src.Carbs)
            .Map(dest => dest.FoodItems, src => src.FoodItems);

        config.NewConfig<FoodItemDto, FoodItem>()
            .MapToConstructor(true);

        config.NewConfig<FoodItem, FoodItemDto>();
    }
}
