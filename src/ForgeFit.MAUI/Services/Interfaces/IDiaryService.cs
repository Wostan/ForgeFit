using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Food;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface IDiaryService
{
    Task<ServiceResponse<List<FoodEntryDto>>> GetEntriesByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<ServiceResponse<List<FoodEntryDto>>> GetEntriesByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<ServiceResponse<FoodEntryDto?>> GetEntryAsync(Guid entryId, CancellationToken cancellationToken = default);
    Task<ServiceResponse<FoodEntryDto>> CreateEntryAsync(FoodEntryCreateRequest request);
    Task<ServiceResponse<FoodEntryDto?>> UpdateEntryAsync(Guid entryId, FoodEntryCreateRequest request);
    Task<ServiceResponse<bool>> DeleteEntryAsync(Guid entryId);
}
