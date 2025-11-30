using ForgeFit.Application.DTOs.Workout;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IWorkoutTrackingService
{
    Task<WorkoutEntryDto> LogEntryAsync(Guid userId, WorkoutEntryDto workoutEntryDto);
    Task<WorkoutEntryDto> UpdateEntryAsync(Guid userId, Guid entryId, WorkoutEntryDto workoutEntryDto);
    Task DeleteEntryAsync(Guid userId, Guid entryId);
    
    Task<WorkoutEntryDto> GetEntryAsync(Guid userId, Guid entryId);
    Task<List<WorkoutEntryDto>> GetEntriesByDateAsync(Guid userId, DateTime date);
    Task<List<WorkoutEntryDto>> GetEntriesByDateAsync(Guid userId, DateTime from, DateTime to);
}