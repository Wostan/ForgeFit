using ForgeFit.Application.DTOs.Food;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IFoodTrackingService
{
    Task<FoodEntryDto> LogFoodEntryAsync(Guid userId, FoodEntryCreateRequest entryDto);
    Task<DrinkEntryResponse> LogDrinkEntryAsync(Guid userId, int volumeMl);
    
    Task<FoodEntryDto> UpdateFoodEntryAsync(Guid userId, Guid entryId, FoodEntryCreateRequest entryDto);
    Task<DrinkEntryResponse> UpdateDrinkEntryAsync(Guid userId, Guid entryId, int volumeMl);
    
    Task DeleteFoodEntryAsync(Guid userId, Guid entryId);
    Task DeleteDrinkEntryAsync(Guid userId, Guid entryId);
    
    Task<FoodEntryDto> GetFoodEntryAsync(Guid userId, Guid entryId);
    Task<List<FoodEntryDto>> GetFoodEntriesByDateAsync(Guid userId, DateTime date);
    Task<List<FoodEntryDto>> GetFoodEntriesByDateAsync(Guid userId, DateTime from, DateTime to);
    
    Task<DrinkEntryResponse> GetDrinkEntryAsync(Guid userId, Guid entryId);
    Task<List<DrinkEntryResponse>> GetDrinkEntriesByDateAsync(Guid userId, DateTime date);
    Task<List<DrinkEntryResponse>> GetDrinkEntriesByDateAsync(Guid userId, DateTime from, DateTime to);
}