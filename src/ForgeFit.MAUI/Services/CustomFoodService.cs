using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class CustomFoodService(IApiService apiService) : ICustomFoodService
{
    public async Task<ServiceResponse<List<CustomFoodDto>>> GetAllForUserAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await apiService.GetAsync<List<CustomFoodDto>>($"/api/custom-food?userId={userId}", cancellationToken);
    }

    public async Task<ServiceResponse<CustomFoodDto>> GetByIdAsync(Guid userId, Guid id,
        CancellationToken cancellationToken = default)
    {
        return await apiService.GetAsync<CustomFoodDto>($"/api/custom-food/{id}?userId={userId}", cancellationToken);
    }

    public async Task<ServiceResponse<CustomFoodDto>> CreateAsync(Guid userId, CustomFoodCreateRequest request)
    {
        return await apiService.PostAsync<CustomFoodCreateRequest, CustomFoodDto>($"/api/custom-food?userId={userId}", request);
    }

    public async Task<ServiceResponse<CustomFoodDto?>> UpdateAsync(Guid userId, Guid id,
        CustomFoodUpdateRequest request)
    {
        return await apiService.PutAsync<CustomFoodUpdateRequest, CustomFoodDto>($"/api/custom-food/{id}?userId={userId}", request);
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(Guid userId, Guid id)
    {
        return await apiService.DeleteAsync($"/api/custom-food/{id}?userId={userId}");
    }
}
