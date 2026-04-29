using ForgeFit.Application.DTOs.Food;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface ICustomFoodService
{
    Task<CustomFoodDto> CreateAsync(Guid userId, CustomFoodCreateRequest request);
    Task<CustomFoodDto> GetByIdAsync(Guid userId, Guid foodId);
    Task<List<CustomFoodDto>> GetAllForUserAsync(Guid userId);
    Task<CustomFoodDto> UpdateAsync(Guid userId, Guid foodId, CustomFoodUpdateRequest request);
    Task DeleteAsync(Guid userId, Guid foodId);
}
