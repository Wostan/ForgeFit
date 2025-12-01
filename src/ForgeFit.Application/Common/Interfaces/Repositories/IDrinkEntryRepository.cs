using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Domain.Aggregates.FoodAggregate;

namespace ForgeFit.Application.Common.Interfaces.Repositories;

public interface IDrinkEntryRepository : IRepository<DrinkEntry>
{
    Task<List<DrinkEntry>> GetAllByUserIdAsync(Guid userId);
    Task<List<DrinkEntry>> GetAllByUserIdAndDateAsync(Guid userId, DateTime date);
    Task<List<DrinkEntry>> GetAllByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
}
