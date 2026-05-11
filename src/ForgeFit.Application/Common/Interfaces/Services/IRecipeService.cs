using ForgeFit.Application.DTOs.Food;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IRecipeService
{
    Task<RecipeDto> CreateAsync(Guid userId, RecipeCreateRequest request);
    Task<RecipeDto> GetByIdAsync(Guid userId, Guid recipeId);
    Task<List<RecipeDto>> GetAllForUserAsync(Guid userId);
    Task<RecipeDto> UpdateAsync(Guid userId, Guid recipeId, RecipeUpdateRequest request);
    Task DeleteAsync(Guid userId, Guid recipeId);
}
