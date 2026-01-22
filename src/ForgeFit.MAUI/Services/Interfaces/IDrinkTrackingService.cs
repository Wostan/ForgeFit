using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.DrinkTracking;
using ForgeFit.MAUI.Models.DTOs.Food;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface IDrinkTrackingService
{
    Task<ServiceResponse<List<DrinkEntryResponse>>> GetEntriesByDateAsync(DateTime date,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse<List<DrinkEntryResponse>>> GetEntriesByDateRangeAsync(DateTime from, DateTime to,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse<DrinkEntryResponse?>> GetEntryAsync(Guid entryId,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse<DrinkEntryResponse>> CreateEntryAsync(DrinkEntryCreateRequest request);
    Task<ServiceResponse<DrinkEntryResponse?>> UpdateEntryAsync(Guid entryId, DrinkEntryCreateRequest request);
    Task<ServiceResponse<bool>> DeleteEntryAsync(Guid entryId);
}
