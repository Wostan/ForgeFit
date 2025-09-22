namespace HabitsDaily.Domain.Aggregates.PostAggregate;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Post post, CancellationToken ct = default);
    Task UpdateAsync(Post post, CancellationToken ct = default);
    Task DeleteAsync(Post post, CancellationToken ct = default);

    Task<Post?> GetByIdWithUserAsync(Guid id, CancellationToken ct = default);
    Task<Post?> GetByIdWithLikesAndCommentsAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<Post>> GetAllAsync(int skip = 0, int take = 20, CancellationToken ct = default);
    Task<IReadOnlyList<Post>> GetAllByUserIdAsync(Guid userId, int skip = 0, int take = 20, CancellationToken ct = default);
    Task<IReadOnlyList<Post>> GetAllByUserIdWithCommentsAsync(Guid userId, int skip = 0, int take = 20, CancellationToken ct = default);
    Task<IReadOnlyList<Post>> GetAllByUserIdWithLikesAsync(Guid userId, int skip = 0, int take = 20, CancellationToken ct = default);
    Task<IReadOnlyList<Post>> GetAllByUserIdWithLikesAndCommentsAsync(Guid userId, int skip = 0, int take = 20, CancellationToken ct = default);

    Task<IReadOnlyList<Post>> GetAllBySearchAsync(string search, int skip = 0, int take = 20, CancellationToken ct = default);

    Task<IReadOnlyList<Post>> GetFeedAsync(Guid userId, int skip = 0, int take = 20, CancellationToken ct = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}