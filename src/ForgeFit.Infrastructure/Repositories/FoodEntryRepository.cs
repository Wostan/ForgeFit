using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Domain.Aggregates.FoodAggregate;
using ForgeFit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ForgeFit.Infrastructure.Repositories;

public class FoodEntryRepository(AppDbContext context) : IFoodEntryRepository
{
    public async Task AddAsync(FoodEntry entity)
    {
        await context.FoodEntries.AddAsync(entity);
    }

    public async Task<FoodEntry?> GetByIdAsync(Guid id)
    {
        return await context.FoodEntries.FindAsync(id);
    }

    public async Task<List<FoodEntry>> GetAllAsync()
    {
        return await context.FoodEntries.ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.FoodEntries.AnyAsync(e => e.Id == id);
    }

    public void Remove(FoodEntry entity)
    {
        context.FoodEntries.Remove(entity);
    }

    public async Task<FoodEntry?> GetByIdWithNavigationsAsync(Guid entryId)
    {
        return await context.FoodEntries
            .Include(fe => fe.FoodItems)
            .FirstOrDefaultAsync(fe => fe.Id == entryId);
    }

    public async Task<List<FoodEntry>> GetAllByUserIdAsync(Guid userId)
    {
        return await context.FoodEntries
            .Where(fe => fe.UserId == userId)
            .OrderByDescending(fe => fe.Date)
            .Include(fe => fe.FoodItems)
            .ToListAsync();
    }

    public async Task<List<FoodEntry>> GetAllByUserIdAndDateAsync(Guid userId, DateTime date)
    {
        return await context.FoodEntries
            .Where(fe => fe.UserId == userId && fe.Date.Date == date.Date)
            .OrderByDescending(fe => fe.Date)
            .Include(fe => fe.FoodItems)
            .ToListAsync();
    }

    public async Task<List<FoodEntry>> GetAllByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await context.FoodEntries
            .Where(fe => fe.UserId == userId && fe.Date.Date >= startDate.Date && fe.Date.Date <= endDate.Date)
            .OrderByDescending(fe => fe.Date)
            .Include(fe => fe.FoodItems)
            .ToListAsync();
    }
}