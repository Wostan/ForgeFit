using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ForgeFit.Infrastructure.Repositories;

public class BodyGoalRepository (AppDbContext dbContext) : IBodyGoalRepository
{
    public async Task AddAsync(BodyGoal entity)
    {
        await dbContext.BodyGoals.AddAsync(entity);
    }

    public async Task<BodyGoal?> GetByIdAsync(Guid id)
    {
        return await dbContext.BodyGoals.FindAsync(id);
    }

    public async Task<List<BodyGoal>> GetAllAsync()
    {
        return await dbContext.BodyGoals.ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await dbContext.BodyGoals.AnyAsync(b => b.Id == id);
    }

    public void Remove(BodyGoal entity)
    {
        dbContext.BodyGoals.Remove(entity);
    }

    public async Task<BodyGoal?> GetByUserIdAsync(Guid userId)
    {
        return await dbContext.BodyGoals
            .FirstOrDefaultAsync(b => b.UserId == userId);
    }
}