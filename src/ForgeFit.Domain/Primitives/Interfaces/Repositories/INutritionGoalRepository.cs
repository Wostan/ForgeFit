using ForgeFit.Domain.Aggregates.GoalAggregate;

namespace ForgeFit.Domain.Primitives.Interfaces.Repositories;

public interface INutritionGoalRepository : IRepository<NutritionGoal>
{
    Task<NutritionGoal?> GetByUserIdAsync(Guid userId);
}