using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Domain.Aggregates.FoodAggregate;
using ForgeFit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ForgeFit.Infrastructure.Repositories;

public class FoodProductRepository(AppDbContext context) : IFoodProductRepository
{
    public async Task AddAsync(FoodProduct entity)
    {
        await context.FoodProducts.AddAsync(entity);
    }

    public async Task<FoodProduct?> GetByIdAsync(Guid id)
    {
        return await context.FoodProducts
            .FirstOrDefaultAsync(fp => fp.Id == id);
    }

    public async Task<List<FoodProduct>> GetAllAsync()
    {
        return await context.FoodProducts
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<FoodProduct>> GetAllByUserIdAsync(Guid userId)
    {
        return await context.FoodProducts
            .Where(fp => fp.UserId == userId)
            .OrderByDescending(fp => fp.CreatedAt)
            .ToListAsync();
    }

    public void Remove(FoodProduct entity)
    {
        context.FoodProducts.Remove(entity);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.FoodProducts.AnyAsync(e => e.Id == id);
    }
}
