using ForgeFit.Domain.Aggregates.UserAggregate;

namespace ForgeFit.Application.Common.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken entity);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<bool> ExistsAsync(Guid id);
    void Remove(RefreshToken entity);
}