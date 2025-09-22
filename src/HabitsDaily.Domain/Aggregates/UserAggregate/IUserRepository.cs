using HabitsDaily.Domain.ValueObjects;

namespace HabitsDaily.Domain.Aggregates.UserAggregate;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);

    Task AddAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
    Task DeleteAsync(User user, CancellationToken ct = default);

    Task<User?> GetByIdWithPostsAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByIdWithHabitsAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByIdWithArchivedUserStatsAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByIdWithPurchasesAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByIdWithFriendsAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByIdWithProfileDataAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<User>> GetAllAsync(int skip = 0, int take = 50, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetAllWithPostsAsync(int skip = 0, int take = 50, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetAllBySearchAsync(string search, int skip = 0, int take = 50, CancellationToken ct = default);

    Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default);
}