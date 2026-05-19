using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class RecipeService(IApiService apiService) : IRecipeService
{
    public async Task<ServiceResponse<List<RecipeDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await apiService.GetAsync<List<RecipeDto>>("/api/recipes", cancellationToken);
    }

    public async Task<ServiceResponse<RecipeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await apiService.GetAsync<RecipeDto>($"/api/recipes/{id}", cancellationToken);
    }

    public async Task<ServiceResponse<RecipeDto>> CreateAsync(RecipeCreateRequest request)
    {
        return await apiService.PostAsync<RecipeCreateRequest, RecipeDto>("/api/recipes", request);
    }

    public async Task<ServiceResponse<RecipeDto?>> UpdateAsync(Guid id, RecipeUpdateRequest request)
    {
        return await apiService.PutAsync<RecipeUpdateRequest, RecipeDto>($"/api/recipes/{id}", request);
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(Guid id)
    {
        return await apiService.DeleteAsync($"/api/recipes/{id}");
    }
}
