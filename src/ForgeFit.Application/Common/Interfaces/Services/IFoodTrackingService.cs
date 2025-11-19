using ForgeFit.Application.DTOs.Food;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IFoodTrackingService
{
    Task<FoodEntryDto> LogEntryAsync(Guid userId, FoodEntryDto entryDto);
    Task<FoodEntryDto> UpdateEntryAsync(Guid userId, Guid entryId, FoodEntryDto entryDto);
    Task DeleteEntryAsync(Guid userId, Guid entryId);
    
    Task<FoodEntryDto> GetEntryAsync(Guid userId, Guid entryId);
    Task<List<FoodEntryDto>> GetEntriesByDateAsync(Guid userId, DateTime date);
    Task<List<FoodEntryDto>> GetEntriesByDateAsync(Guid userId, DateTime from, DateTime to);
}