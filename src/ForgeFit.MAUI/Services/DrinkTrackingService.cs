using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class DrinkTrackingService(IApiService apiService) : IDrinkTrackingService
{
    public async Task<ServiceResponse<List<DrinkEntryResponse>>> GetEntriesByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var dateStr = date.ToString("yyyy-MM-ddTHH:mm:ss"); 
        return await apiService.GetAsync<List<DrinkEntryResponse>>($"/api/drink-tracking/entries/by-date?date={dateStr}", cancellationToken);
    }

    public async Task<ServiceResponse<List<DrinkEntryResponse>>> GetEntriesByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var fromStr = from.ToString("yyyy-MM-ddTHH:mm:ss");
        var toStr = to.ToString("yyyy-MM-ddTHH:mm:ss");
        return await apiService.GetAsync<List<DrinkEntryResponse>>($"/api/drink-tracking/entries/by-date?from={fromStr}&to={toStr}", cancellationToken);
    }

    public async Task<ServiceResponse<DrinkEntryResponse?>> GetEntryAsync(Guid entryId, CancellationToken cancellationToken = default)
    {
        return await apiService.GetAsync<DrinkEntryResponse?>($"/api/drink-tracking/entries/{entryId}", cancellationToken);
    }

    public async Task<ServiceResponse<DrinkEntryResponse>> CreateEntryAsync(DrinkEntryCreateRequest request)
    {
        return await apiService.PostAsync<DrinkEntryCreateRequest, DrinkEntryResponse>("/api/drink-tracking/entries", request);
    }

    public async Task<ServiceResponse<DrinkEntryResponse?>> UpdateEntryAsync(Guid entryId, DrinkEntryCreateRequest request)
    {
        return await apiService.PutAsync<DrinkEntryCreateRequest, DrinkEntryResponse>($"/api/drink-tracking/entries/{entryId}", request);
    }

    public async Task<ServiceResponse<bool>> DeleteEntryAsync(Guid entryId)
    {
        return await apiService.DeleteAsync($"/api/drink-tracking/entries/{entryId}");
    }
}
