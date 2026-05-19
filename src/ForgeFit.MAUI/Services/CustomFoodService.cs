using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class CustomFoodService(IApiService apiService) : ICustomFoodService
{
    public async Task<ServiceResponse<List<CustomFoodDto>>> GetAllForUserAsync(
        CancellationToken cancellationToken = default)
    {
        return await apiService.GetAsync<List<CustomFoodDto>>("/api/custom-food", cancellationToken);
    }

    public async Task<ServiceResponse<CustomFoodDto>> GetByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return await apiService.GetAsync<CustomFoodDto>($"/api/custom-food/{id}", cancellationToken);
    }

    public async Task<ServiceResponse<CustomFoodDto>> CreateAsync(CustomFoodCreateRequest request)
    {
        return await apiService.PostAsync<CustomFoodCreateRequest, CustomFoodDto>("/api/custom-food", request);
    }

    public async Task<ServiceResponse<CustomFoodDto?>> UpdateAsync(Guid id, CustomFoodUpdateRequest request)
    {
        return await apiService.PutAsync<CustomFoodUpdateRequest, CustomFoodDto>($"/api/custom-food/{id}", request);
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(Guid id)
    {
        return await apiService.DeleteAsync($"/api/custom-food/{id}");
    }
}
