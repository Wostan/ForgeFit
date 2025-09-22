namespace HabitsDaily.Domain.Aggregates.StreakAggregate;

public interface IStreakRepository
{
    Task<Streak?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Streak?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Streak streak, CancellationToken ct = default);
    Task UpdateAsync(Streak streak, CancellationToken ct = default);
    Task DeleteAsync(Streak streak, CancellationToken ct = default);

    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken ct = default);
}