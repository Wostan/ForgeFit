using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ForgeFit.Infrastructure.Repositories;

public class WorkoutGoalRepository(AppDbContext dbContext) : IWorkoutGoalRepository
{
    public async Task AddAsync(WorkoutGoal entity)
    {
        await dbContext.WorkoutGoals.AddAsync(entity);
    }

    public async Task<WorkoutGoal?> GetByIdAsync(Guid id)
    {
        return await dbContext.WorkoutGoals.FindAsync(id);
    }

    public async Task<List<WorkoutGoal>> GetAllAsync()
    {
        return await dbContext.WorkoutGoals.ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await dbContext.WorkoutGoals.AnyAsync(w => w.Id == id);
    }

    public void Remove(WorkoutGoal entity)
    {
        dbContext.WorkoutGoals.Remove(entity);
    }

    public async Task<WorkoutGoal?> GetByUserIdAsync(Guid userId)
    {
        return await dbContext.WorkoutGoals
            .FirstOrDefaultAsync(w => w.UserId == userId);
    }
}