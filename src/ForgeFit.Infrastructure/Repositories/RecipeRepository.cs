using ForgeFit.Application.Common.Interfaces.Repositories;
using ForgeFit.Domain.Aggregates.FoodAggregate;
using ForgeFit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ForgeFit.Infrastructure.Repositories;

public class RecipeRepository(AppDbContext context) : IRecipeRepository
{
    public async Task AddAsync(Recipe entity)
    {
        await context.Recipes.AddAsync(entity);
    }

    public async Task<Recipe?> GetByIdAsync(Guid id)
    {
        return await context.Recipes
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Recipe>> GetAllAsync()
    {
        return await context.Recipes
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Recipe>> GetAllByUserIdAsync(Guid userId)
    {
        return await context.Recipes
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Include(r => r.Ingredients)
            .ToListAsync();
    }

    public void Remove(Recipe entity)
    {
        context.Recipes.Remove(entity);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.Recipes.AnyAsync(e => e.Id == id);
    }

    public async Task<int> CountByUserIdAsync(Guid userId)
    {
        return await context.Recipes.CountAsync(r => r.UserId == userId);
    }
}
