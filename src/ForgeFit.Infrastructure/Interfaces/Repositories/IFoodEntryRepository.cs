using ForgeFit.Domain.Aggregates.FoodAggregate;

namespace ForgeFit.Infrastructure.Interfaces.Repositories;

public interface IFoodEntryRepository : IRepository<FoodEntry>
{
    Task<FoodEntry?> GetByUserIdAsync(Guid userId);
    Task<List<FoodEntry>> GetAllByUserIdAsync(Guid userId);
    Task<List<FoodEntry>> GetAllByUserIdAndDateAsync(Guid userId, DateTime date);
    Task<List<FoodEntry>> GetAllByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
}