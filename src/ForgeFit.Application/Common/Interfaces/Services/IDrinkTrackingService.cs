using ForgeFit.Application.DTOs.Food;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IDrinkTrackingService
{
    public Task<DrinkEntryResponse> LogDrinkEntryAsync(Guid userId, DrinkEntryCreateRequest request);
    public Task<DrinkEntryResponse> UpdateDrinkEntryAsync(Guid userId, Guid entryId, DrinkEntryCreateRequest request);
    public Task DeleteDrinkEntryAsync(Guid userId, Guid entryId);

    public Task<DrinkEntryResponse> GetDrinkEntryAsync(Guid userId, Guid entryId);
    public Task<List<DrinkEntryResponse>> GetDrinkEntriesByDateAsync(Guid userId, DateTime date);
    public Task<List<DrinkEntryResponse>> GetDrinkEntriesByDateAsync(Guid userId, DateTime from, DateTime to);
}