using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class DiaryService(IApiService apiService) : IDiaryService
{
    public async Task<ServiceResponse<List<FoodEntryDto>>> GetEntriesByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var dateStr = date.ToString("yyyy-MM-ddTHH:mm:ss"); 
        return await apiService.GetAsync<List<FoodEntryDto>>($"/api/food-tracking/entries/by-date?date={dateStr}", cancellationToken);
    }

    public async Task<ServiceResponse<List<FoodEntryDto>>> GetEntriesByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var fromStr = from.ToString("yyyy-MM-ddTHH:mm:ss");
        var toStr = to.ToString("yyyy-MM-ddTHH:mm:ss");
        return await apiService.GetAsync<List<FoodEntryDto>>($"/api/food-tracking/entries/by-date?from={fromStr}&to={toStr}", cancellationToken);
    }

    public async Task<ServiceResponse<FoodEntryDto?>> GetEntryAsync(Guid entryId, CancellationToken cancellationToken = default)
    {
        return await apiService.GetAsync<FoodEntryDto?>($"/api/food-tracking/entries/{entryId}", cancellationToken);
    }

    public async Task<ServiceResponse<FoodEntryDto>> CreateEntryAsync(FoodEntryCreateRequest request)
    {
        return await apiService.PostAsync<FoodEntryCreateRequest, FoodEntryDto>("/api/food-tracking/entries", request);
    }

    public async Task<ServiceResponse<FoodEntryDto?>> UpdateEntryAsync(Guid entryId, FoodEntryCreateRequest request)
    {
        return await apiService.PutAsync<FoodEntryCreateRequest, FoodEntryDto>($"/api/food-tracking/entries/{entryId}", request);
    }

    public async Task<ServiceResponse<bool>> DeleteEntryAsync(Guid entryId)
    {
        var response = await apiService.DeleteAsync($"/api/food-tracking/entries/{entryId}");
        return response.Success 
            ? ServiceResponse<bool>.Ok(true) 
            : ServiceResponse<bool>.Error(response.Message, response.StatusCode);
    }
}
