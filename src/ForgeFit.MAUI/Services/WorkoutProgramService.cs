using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class WorkoutProgramService(IApiService apiService) : IWorkoutProgramService
{
    private const string BaseUrl = "/api/workout-program";

    public async Task<ServiceResponse<List<WorkoutProgramResponse>>> GetAllProgramsAsync()
    {
        return await apiService.GetAsync<List<WorkoutProgramResponse>>(BaseUrl);
    }

    public async Task<ServiceResponse<WorkoutProgramResponse>> GetProgramAsync(Guid id)
    {
        return await apiService.GetAsync<WorkoutProgramResponse>($"{BaseUrl}/{id}");
    }

    public async Task<ServiceResponse<WorkoutProgramResponse>> CreateProgramAsync(WorkoutProgramRequest request)
    {
        return await apiService.PostAsync<WorkoutProgramRequest, WorkoutProgramResponse>(BaseUrl, request);
    }

    public async Task<ServiceResponse<WorkoutProgramResponse?>> UpdateProgramAsync(Guid id,
        WorkoutProgramRequest request)
    {
        return await apiService.PutAsync<WorkoutProgramRequest, WorkoutProgramResponse>($"{BaseUrl}/{id}", request);
    }

    public async Task<ServiceResponse<bool>> DeleteProgramAsync(Guid id)
    {
        var response = await apiService.DeleteAsync($"{BaseUrl}/{id}");
        return response.Success
            ? ServiceResponse<bool>.Ok(true)
            : ServiceResponse<bool>.Error(response.Message, response.StatusCode);
    }
}
