using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Domain.Aggregates.FoodAggregate;

namespace ForgeFit.Application.Common.Interfaces.Repositories;

public interface IFoodProductRepository : IRepository<FoodProduct>
{
    Task<List<FoodProduct>> GetAllByUserIdAsync(Guid userId);
}
