using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Food;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface ICustomFoodService
{
    Task<ServiceResponse<List<CustomFoodDto>>> GetAllForUserAsync(Guid userId,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse<CustomFoodDto>> GetByIdAsync(Guid userId, Guid id,
        CancellationToken cancellationToken = default);
    
    Task<ServiceResponse<CustomFoodDto>> CreateAsync(Guid userId, CustomFoodCreateRequest request);
    Task<ServiceResponse<CustomFoodDto?>> UpdateAsync(Guid userId, Guid id, CustomFoodUpdateRequest request);
    Task<ServiceResponse<bool>> DeleteAsync(Guid userId, Guid id);
}