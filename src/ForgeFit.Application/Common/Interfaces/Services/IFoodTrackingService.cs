using ForgeFit.Application.DTOs.Food;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IFoodTrackingService
{
    Task<FoodEntryDto> LogFoodEntryAsync(Guid userId, FoodEntryCreateRequest request);
    Task<FoodEntryDto> UpdateFoodEntryAsync(Guid userId, Guid entryId, FoodEntryCreateRequest request);
    Task DeleteFoodEntryAsync(Guid userId, Guid entryId);
    
    Task<FoodEntryDto> GetFoodEntryAsync(Guid userId, Guid entryId);
    Task<List<FoodEntryDto>> GetFoodEntriesByDateAsync(Guid userId, DateTime date);
    Task<List<FoodEntryDto>> GetFoodEntriesByDateAsync(Guid userId, DateTime from, DateTime to);
}