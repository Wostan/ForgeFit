using ForgeFit.Domain.Aggregates.HabitAggregate;

namespace ForgeFit.Domain.Primitives.Interfaces.Repositories;

public interface IHabitRecordRepository : IRepository<HabitRecord>
{
    Task<List<HabitRecord>> GetAllByUserIdAsync(Guid userId);
    Task<List<HabitRecord>> GetAllByUserIdAndHabitIdAsync(Guid userId, Guid habitId);
    Task<List<HabitRecord>> GetAllByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task<List<HabitRecord>> GetAllByUserIdAndHabitIdAndDateRangeAsync(
        Guid userId, Guid habitId, DateTime startDate, DateTime endDate);
}