using ForgeFit.Domain.Aggregates.GoalAggregate;

namespace ForgeFit.Infrastructure.Interfaces.Repositories;

public interface INutritionGoalRepository : IRepository<NutritionGoal>
{
    Task<NutritionGoal?> GetByUserIdAsync(Guid userId);
}