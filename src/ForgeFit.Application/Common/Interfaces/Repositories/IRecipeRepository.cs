using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Domain.Aggregates.FoodAggregate;

namespace ForgeFit.Application.Common.Interfaces.Repositories;

public interface IRecipeRepository : IRepository<Recipe>
{
    Task<List<Recipe>> GetAllByUserIdAsync(Guid userId);
    Task<int> CountByUserIdAsync(Guid userId);
}
