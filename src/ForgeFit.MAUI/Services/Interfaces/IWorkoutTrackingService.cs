using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Workout;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface IWorkoutTrackingService
{
    Task<ServiceResponse<List<WorkoutEntryDto>>> GetEntriesByDateAsync(DateTime date,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse<List<WorkoutEntryDto>>> GetEntriesByDateRangeAsync(DateTime from, DateTime to,
        CancellationToken cancellationToken = default);

    Task<ServiceResponse<WorkoutEntryDto>> GetEntryAsync(Guid entryId, CancellationToken cancellationToken = default);

    Task<ServiceResponse<WorkoutEntryDto>> LogEntryAsync(WorkoutEntryDto entryDto);

    Task<ServiceResponse<WorkoutEntryDto?>> UpdateEntryAsync(Guid entryId, WorkoutEntryDto entryDto);

    Task<ServiceResponse<bool>> DeleteEntryAsync(Guid entryId);
}
