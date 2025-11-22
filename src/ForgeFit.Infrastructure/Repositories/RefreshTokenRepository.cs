using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ForgeFit.Infrastructure.Repositories;

public class RefreshTokenRepository(AppDbContext dbContext) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken entity)
    {
        await dbContext.RefreshTokens.AddAsync(entity);
    }
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await dbContext.RefreshTokens.AnyAsync(rt => rt.Id == id);
    }

    public void Remove(RefreshToken entity)
    { 
        dbContext.RefreshTokens.Remove(entity);
    }
}