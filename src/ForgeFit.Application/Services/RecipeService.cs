using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Application.DTOs.Food;
using ForgeFit.Domain.Aggregates.FoodAggregate;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.ValueObjects.FoodValueObjects;
using MapsterMapper;

namespace ForgeFit.Application.Services;

public class RecipeService(
    IRecipeRepository recipeRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRecipeService
{
    public async Task<RecipeDto> CreateAsync(Guid userId, RecipeCreateRequest request)
    {
        if (!await userRepository.ExistsAsync(userId)) throw new NotFoundException("User not found");

        var recipeCount = await recipeRepository.CountByUserIdAsync(userId);
        if (recipeCount >= DomainConstants.ValidationLimits.MaxRecipesPerUser) 
            throw new BadRequestException($"Cannot exceed {DomainConstants.ValidationLimits.MaxRecipesPerUser} recipes per user");

        var ingredients = GetFoodItemsFromDto(request.Ingredients);

        var recipe = Recipe.Create(
            userId,
            request.Name,
            request.Description,
            ingredients
        );

        await recipeRepository.AddAsync(recipe);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<RecipeDto>(recipe);
    }

    public async Task<RecipeDto> GetByIdAsync(Guid userId, Guid recipeId)
    {
        var recipe = await recipeRepository.GetByIdAsync(recipeId);

        if (recipe == null) throw new NotFoundException("Recipe not found");

        if (userId != recipe.UserId) throw new UnauthorizedAccessException("You do not own this recipe");

        return mapper.Map<RecipeDto>(recipe);
    }

    public async Task<List<RecipeDto>> GetAllForUserAsync(Guid userId)
    {
        var recipes = await recipeRepository.GetAllByUserIdAsync(userId);
        return mapper.Map<List<RecipeDto>>(recipes);
    }

    public async Task<RecipeDto> UpdateAsync(Guid userId, Guid recipeId, RecipeUpdateRequest request)
    {
        var recipe = await recipeRepository.GetByIdAsync(recipeId);

        if (recipe == null) throw new NotFoundException("Recipe not found");

        if (userId != recipe.UserId) throw new UnauthorizedAccessException("You do not own this recipe");

        var ingredients = GetFoodItemsFromDto(request.Ingredients);

        recipe.UpdateDetails(request.Name, request.Description);
        recipe.UpdateIngredients(ingredients);

        await unitOfWork.SaveChangesAsync();

        return mapper.Map<RecipeDto>(recipe);
    }

    public async Task DeleteAsync(Guid userId, Guid recipeId)
    {
        var recipe = await recipeRepository.GetByIdAsync(recipeId);

        if (recipe == null) throw new NotFoundException("Recipe not found");

        if (userId != recipe.UserId) throw new UnauthorizedAccessException("You do not own this recipe");

        recipeRepository.Remove(recipe);
        await unitOfWork.SaveChangesAsync();
    }

    private static HashSet<FoodItem> GetFoodItemsFromDto(List<FoodItemDto> foodItemDtos)
    {
        return foodItemDtos.Select(dto => new FoodItem(
            dto.ExternalId,
            dto.Label,
            dto.Calories,
            dto.Carbs,
            dto.Protein,
            dto.Fat,
            dto.Fiber,
            dto.Sugar,
            dto.SaturatedFat,
            dto.Sodium,
            dto.ServingUnit,
            dto.Amount
        )).ToHashSet();
    }
}
