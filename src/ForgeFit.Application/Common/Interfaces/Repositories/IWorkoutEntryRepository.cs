using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Domain.Aggregates.WorkoutAggregate;

namespace ForgeFit.Application.Common.Interfaces.Repositories;

public interface IWorkoutEntryRepository : IRepository<WorkoutEntry>
{
    Task<List<WorkoutEntry>> GetAllByUserIdAsync(Guid userId);
    Task<List<WorkoutEntry>> GetAllByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
}