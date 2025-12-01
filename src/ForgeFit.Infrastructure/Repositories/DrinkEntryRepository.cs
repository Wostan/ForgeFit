using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Domain.Aggregates.FoodAggregate;
using ForgeFit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ForgeFit.Infrastructure.Repositories;

public class DrinkEntryRepository(AppDbContext context) : IDrinkEntryRepository
{
    public async Task AddAsync(DrinkEntry entity)
    {
        await context.DrinkEntries.AddAsync(entity);
    }

    public async Task<DrinkEntry?> GetByIdAsync(Guid id)
    {
        return await context.DrinkEntries.FindAsync(id);
    }

    public async Task<List<DrinkEntry>> GetAllAsync()
    {
        return await context.DrinkEntries.ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.DrinkEntries.AnyAsync(e => e.Id == id);
    }

    public void Remove(DrinkEntry entity)
    {
        context.DrinkEntries.Remove(entity);
    }

    public async Task<List<DrinkEntry>> GetAllByUserIdAsync(Guid userId)
    {
        return await context.DrinkEntries.Where(de => de.UserId == userId).ToListAsync();
    }

    public async Task<List<DrinkEntry>> GetAllByUserIdAndDateAsync(Guid userId, DateTime date)
    {
        return await context.DrinkEntries
            .Where(de => de.UserId == userId && de.Date.Date == date.Date)
            .ToListAsync();
    }

    public async Task<List<DrinkEntry>> GetAllByUserIdAndDateRangeAsync(Guid userId, DateTime startDate,
        DateTime endDate)
    {
        return await context.DrinkEntries
            .Where(de => de.UserId == userId && de.Date.Date >= startDate.Date && de.Date.Date <= endDate.Date)
            .ToListAsync();
    }
}
