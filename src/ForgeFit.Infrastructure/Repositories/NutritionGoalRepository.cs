using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ForgeFit.Infrastructure.Repositories;

public class NutritionGoalRepository(AppDbContext dbContext) : INutritionGoalRepository
{
    public async Task AddAsync(NutritionGoal entity)
    {
        await dbContext.NutritionGoals.AddAsync(entity);
    }

    public async Task<NutritionGoal?> GetByIdAsync(Guid id)
    {
        return await dbContext.NutritionGoals.FindAsync(id);
    }

    public async Task<List<NutritionGoal>> GetAllAsync()
    {
        return await dbContext.NutritionGoals.ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await dbContext.NutritionGoals.AnyAsync(n => n.Id == id);
    }

    public void Remove(NutritionGoal entity)
    {
        dbContext.NutritionGoals.Remove(entity);
    }

    public async Task<NutritionGoal?> GetByUserIdAsync(Guid userId)
    {
        return await dbContext.NutritionGoals
            .FirstOrDefaultAsync(n => n.UserId == userId);
    }
}
