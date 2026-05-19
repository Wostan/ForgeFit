using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Food;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface ICustomFoodService
{
    Task<ServiceResponse<List<CustomFoodDto>>> GetAllForUserAsync(CancellationToken cancellationToken = default);
    Task<ServiceResponse<CustomFoodDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceResponse<CustomFoodDto>> CreateAsync(CustomFoodCreateRequest request);
    Task<ServiceResponse<CustomFoodDto?>> UpdateAsync(Guid id, CustomFoodUpdateRequest request);
    Task<ServiceResponse<bool>> DeleteAsync(Guid id);
}
