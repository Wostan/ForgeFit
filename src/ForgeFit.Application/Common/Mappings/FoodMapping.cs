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
            .Map(dest => dest.TotalCalories, src => src.NutritionInfo.Calories)
            .Map(dest => dest.TotalCarbs, src => src.NutritionInfo.Carbs)
            .Map(dest => dest.TotalProtein, src => src.NutritionInfo.Protein)
            .Map(dest => dest.TotalFat, src => src.NutritionInfo.Fat)
            .Map(dest => dest.TotalFiber, src => src.NutritionInfo.Fiber)
            .Map(dest => dest.TotalSugar, src => src.NutritionInfo.Sugar)
            .Map(dest => dest.TotalSaturatedFat, src => src.NutritionInfo.SaturatedFat)
            .Map(dest => dest.TotalSodium, src => src.NutritionInfo.Sodium)
            .Map(dest => dest.FoodItems, src => src.FoodItems);

        config.NewConfig<FoodItem, FoodItemDto>();

        config.NewConfig<Recipe, RecipeDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
            .Map(dest => dest.Ingredients, src => src.Ingredients)
            .Map(dest => dest.TotalCalories, src => src.TotalNutrition.Calories)
            .Map(dest => dest.TotalCarbs, src => src.TotalNutrition.Carbs)
            .Map(dest => dest.TotalProtein, src => src.TotalNutrition.Protein)
            .Map(dest => dest.TotalFat, src => src.TotalNutrition.Fat)
            .Map(dest => dest.TotalFiber, src => src.TotalNutrition.Fiber)
            .Map(dest => dest.TotalSugar, src => src.TotalNutrition.Sugar)
            .Map(dest => dest.TotalSaturatedFat, src => src.TotalNutrition.SaturatedFat)
            .Map(dest => dest.TotalSodium, src => src.TotalNutrition.Sodium);

        config.NewConfig<FoodProduct, CustomFoodDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.ExternalId, src => src.ExternalId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Brand, src => src.Brand)
            .Map(dest => dest.Barcode, src => src.Barcode)
            .Map(dest => dest.Calories, src => src.Calories)
            .Map(dest => dest.Carbs, src => src.Carbs)
            .Map(dest => dest.Protein, src => src.Protein)
            .Map(dest => dest.Fat, src => src.Fat)
            .Map(dest => dest.Fiber, src => src.Fiber)
            .Map(dest => dest.Sugar, src => src.Sugar)
            .Map(dest => dest.SaturatedFat, src => src.SaturatedFat)
            .Map(dest => dest.Sodium, src => src.Sodium)
            .Map(dest => dest.ServingSize, src => src.ServingSize)
            .Map(dest => dest.ServingUnit, src => src.ServingUnit)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt);
    }
}
