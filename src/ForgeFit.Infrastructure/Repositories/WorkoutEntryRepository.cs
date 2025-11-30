using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Domain.Aggregates.WorkoutAggregate;
using ForgeFit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ForgeFit.Infrastructure.Repositories;

public class WorkoutEntryRepository(AppDbContext context) : IWorkoutEntryRepository
{
    public async Task AddAsync(WorkoutEntry entity)
    {
        await context.WorkoutEntries.AddAsync(entity);
    }

    public async Task<WorkoutEntry?> GetByIdAsync(Guid id)
    {
        return await context.WorkoutEntries.FindAsync(id);
    }

    public async Task<List<WorkoutEntry>> GetAllAsync()
    {
        return await context.WorkoutEntries.ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.WorkoutEntries.AnyAsync(e => e.Id == id);
    }

    public void Remove(WorkoutEntry entity)
    {
        context.WorkoutEntries.Remove(entity);
    }

    public async Task<WorkoutEntry?> GetByIdWithNavigationsAsync(Guid id)
    {
        return await context.WorkoutEntries
            .Include(we => we.PerformedExercises)
            .ThenInclude(pe => pe.Sets)
            .FirstOrDefaultAsync(we => we.Id == id);
    }

    public async Task<List<WorkoutEntry>> GetAllByUserIdAsync(Guid userId)
    {
        return await context.WorkoutEntries
            .Where(we => we.UserId == userId)
            .OrderByDescending(we => we.CreatedAt)
            .Include(we => we.PerformedExercises)
            .ThenInclude(pe => pe.Sets)
            .ToListAsync();
    }

    public async Task<List<WorkoutEntry>> GetAllByUserIdAndDateAsync(Guid userId, DateTime date)
    {
        return await  context.WorkoutEntries
            .Where(we => we.UserId == userId && we.CreatedAt.Date == date.Date)
            .Include(we => we.PerformedExercises)
            .ThenInclude(pe => pe.Sets)
            .ToListAsync();
    }

    public async Task<List<WorkoutEntry>> GetAllByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await context.WorkoutEntries
            .Where(we => we.UserId == userId 
                         && we.CreatedAt.Date >= startDate.Date 
                         && we.CreatedAt.Date <= endDate.Date)
            .Include(we => we.PerformedExercises)
            .ThenInclude(pe => pe.Sets)
            .OrderByDescending(we => we.CreatedAt)
            .ToListAsync();
    }
}