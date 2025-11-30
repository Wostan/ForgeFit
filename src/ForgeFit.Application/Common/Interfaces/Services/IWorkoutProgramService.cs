using ForgeFit.Application.DTOs.Workout;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IWorkoutProgramService
{
    Task<WorkoutProgramResponse> CreateWorkoutProgramAsync(Guid userId, WorkoutProgramRequest workoutProgramResponse);
    Task<WorkoutProgramResponse> UpdateWorkoutProgramAsync(Guid userId, Guid workoutProgramId, WorkoutProgramRequest workoutProgramResponse);
    Task DeleteWorkoutProgramAsync(Guid userId, Guid workoutProgramId);
    
    Task<List<WorkoutProgramResponse>> GetAllWorkoutProgramsAsync(Guid userId);
    Task<WorkoutProgramResponse> GetWorkoutProgramAsync(Guid userId, Guid workoutProgramId);
}