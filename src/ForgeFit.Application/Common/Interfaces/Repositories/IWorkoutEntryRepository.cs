using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Domain.Aggregates.WorkoutAggregate;

namespace ForgeFit.Application.Common.Interfaces.Repositories;

public interface IWorkoutEntryRepository : IRepository<WorkoutEntry>
{
    Task<WorkoutEntry?> GetByIdWithNavigationsAsync(Guid id);
    Task<List<WorkoutEntry>> GetAllByUserIdAsync(Guid userId);
    Task<List<WorkoutEntry>> GetAllByUserIdAndDateAsync(Guid userId, DateTime date);
    Task<List<WorkoutEntry>> GetAllByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
}