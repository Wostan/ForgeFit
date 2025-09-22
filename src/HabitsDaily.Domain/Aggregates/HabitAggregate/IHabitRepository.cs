using HabitsDaily.Domain.Aggregates.UserAggregate;

namespace HabitsDaily.Domain.Aggregates.HabitAggregate;

public interface IHabitRepository
{
    Task<Habit?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Habit habit, CancellationToken ct = default);
    Task UpdateAsync(Habit habit, CancellationToken ct = default);
    Task DeleteAsync(Habit habit, CancellationToken ct = default);

    Task AddArchivedStatAsync(Guid habitId, ArchivedUserStats stats, CancellationToken ct = default);
    Task UpdateArchivedStatAsync(ArchivedUserStats stats, CancellationToken ct = default);
    Task DeleteArchivedStatAsync(ArchivedUserStats stats, CancellationToken ct = default);

    Task<Habit?> GetByIdWithRecordsAsync(Guid id, CancellationToken ct = default);
    Task<Habit?> GetByIdWithArchivedStatsAsync(Guid id, CancellationToken ct = default);
    Task<Habit?> GetByIdWithUserAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<Habit>> GetAllByUserIdAsync(Guid userId, int skip = 0, int take = 50, CancellationToken ct = default);
    Task<IReadOnlyList<Habit>> GetAllByUserIdWithRecordsAsync(Guid userId, int skip = 0, int take = 50, CancellationToken ct = default);
    Task<IReadOnlyList<Habit>> GetAllByUserIdWithArchivedStatsAsync(Guid userId, int skip = 0, int take = 50, CancellationToken ct = default);

    Task<IReadOnlyList<Habit>> SearchByNameAsync(string name, int skip = 0, int take = 50, CancellationToken ct = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}