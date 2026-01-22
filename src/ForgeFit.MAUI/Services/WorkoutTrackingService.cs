using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class WorkoutTrackingService(IApiService apiService) : IWorkoutTrackingService
{
    private const string BaseUrl = "/api/workout-tracking/entries";

    public async Task<ServiceResponse<List<WorkoutEntryDto>>> GetEntriesByDateAsync(DateTime date,
        CancellationToken cancellationToken = default)
    {
        var dateStr = date.ToString("yyyy-MM-ddTHH:mm:ss");
        return await apiService.GetAsync<List<WorkoutEntryDto>>($"{BaseUrl}/by-date?date={dateStr}",
            cancellationToken);
    }

    public async Task<ServiceResponse<List<WorkoutEntryDto>>> GetEntriesByDateRangeAsync(DateTime from, DateTime to,
        CancellationToken cancellationToken = default)
    {
        var fromStr = from.ToString("yyyy-MM-ddTHH:mm:ss");
        var toStr = to.ToString("yyyy-MM-ddTHH:mm:ss");
        return await apiService.GetAsync<List<WorkoutEntryDto>>(
            $"{BaseUrl}/by-date?from={fromStr}&to={toStr}", cancellationToken);
    }

    public async Task<ServiceResponse<WorkoutEntryDto>> GetEntryAsync(Guid entryId,
        CancellationToken cancellationToken = default)
    {
        return await apiService.GetAsync<WorkoutEntryDto>($"{BaseUrl}/{entryId}", cancellationToken);
    }

    public async Task<ServiceResponse<WorkoutEntryDto>> LogEntryAsync(WorkoutEntryDto entryDto)
    {
        return await apiService.PostAsync<WorkoutEntryDto, WorkoutEntryDto>(BaseUrl, entryDto);
    }

    public async Task<ServiceResponse<WorkoutEntryDto?>> UpdateEntryAsync(Guid entryId, WorkoutEntryDto entryDto)
    {
        return await apiService.PutAsync<WorkoutEntryDto, WorkoutEntryDto>($"{BaseUrl}/{entryId}", entryDto);
    }

    public async Task<ServiceResponse<bool>> DeleteEntryAsync(Guid entryId)
    {
        var response = await apiService.DeleteAsync($"{BaseUrl}/{entryId}");
        return response.Success
            ? ServiceResponse<bool>.Ok(true)
            : ServiceResponse<bool>.Error(response.Message, response.StatusCode);
    }
}
