using ForgeFit.Application.DTOs.Workout;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IWorkoutProgramService
{
    Task<WorkoutProgramDto> CreateWorkoutProgramAsync(Guid userId, WorkoutProgramDto workoutProgramDto);
    Task<WorkoutProgramDto> UpdateWorkoutProgramAsync(Guid userId, WorkoutProgramDto workoutProgramDto);
    Task DeleteWorkoutProgramAsync(Guid userId, Guid workoutProgramId);
    
    Task<List<WorkoutProgramDto>> GetAllWorkoutProgramsAsync(Guid userId);
    Task<WorkoutProgramDto> GetWorkoutProgramAsync(Guid userId, Guid workoutProgramId);
}