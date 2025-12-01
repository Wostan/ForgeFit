using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Domain.Aggregates.WorkoutAggregate;
using ForgeFit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ForgeFit.Infrastructure.Repositories;

public class WorkoutProgramRepository(AppDbContext context) : IWorkoutProgramRepository
{
    public async Task AddAsync(WorkoutProgram entity)
    {
        await context.WorkoutPrograms.AddAsync(entity);
    }

    public async Task<WorkoutProgram?> GetByIdAsync(Guid id)
    {
        return await context.WorkoutPrograms.FindAsync(id);
    }

    public async Task<List<WorkoutProgram>> GetAllAsync()
    {
        return await context.WorkoutPrograms.ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.WorkoutPrograms.AnyAsync(wp => wp.Id == id);
    }

    public void Remove(WorkoutProgram entity)
    {
        context.WorkoutPrograms.Remove(entity);
    }

    public async Task<WorkoutProgram?> GetByIdWithNavigationsAsync(Guid id)
    {
        return await context.WorkoutPrograms
            .Include(wp => wp.WorkoutExercisePlans)
            .ThenInclude(wep => wep.WorkoutSets)
            .FirstOrDefaultAsync(wp => wp.Id == id);
    }

    public async Task<List<WorkoutProgram>> GetAllByUserIdAsync(Guid userId)
    {
        return await context.WorkoutPrograms
            .Where(wp => wp.UserId == userId)
            .OrderByDescending(wp => wp.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<WorkoutProgram>> GetAllByUserIdAndWorkoutProgramNameAsync(Guid userId,
        string workoutProgramName)
    {
        return await context.WorkoutPrograms
            .Where(wp => wp.UserId == userId && wp.Name.Contains(workoutProgramName))
            .OrderByDescending(wp => wp.CreatedAt)
            .ToListAsync();
    }
}
