using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Workout;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface IWorkoutProgramService
{
    Task<ServiceResponse<List<WorkoutProgramResponse>>> GetAllProgramsAsync();
    Task<ServiceResponse<WorkoutProgramResponse>> GetProgramAsync(Guid id);
    Task<ServiceResponse<WorkoutProgramResponse>> CreateProgramAsync(WorkoutProgramRequest request);
    Task<ServiceResponse<WorkoutProgramResponse?>> UpdateProgramAsync(Guid id, WorkoutProgramRequest request);
    Task<ServiceResponse<bool>> DeleteProgramAsync(Guid id);
}
