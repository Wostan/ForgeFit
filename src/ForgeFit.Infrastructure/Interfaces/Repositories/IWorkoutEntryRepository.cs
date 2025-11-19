using ForgeFit.Domain.Aggregates.WorkoutAggregate;

namespace ForgeFit.Infrastructure.Interfaces.Repositories;

public interface IWorkoutEntryRepository : IRepository<WorkoutEntry>
{
    Task<List<WorkoutEntry>> GetAllByUserIdAsync(Guid userId);
    Task<List<WorkoutEntry>> GetAllByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
}