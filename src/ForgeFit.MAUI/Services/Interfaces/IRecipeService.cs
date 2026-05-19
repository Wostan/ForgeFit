using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Food;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface IRecipeService
{
    Task<ServiceResponse<List<RecipeDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ServiceResponse<RecipeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceResponse<RecipeDto>> CreateAsync(RecipeCreateRequest request);
    Task<ServiceResponse<RecipeDto?>> UpdateAsync(Guid id, RecipeUpdateRequest request);
    Task<ServiceResponse<bool>> DeleteAsync(Guid id);
}
