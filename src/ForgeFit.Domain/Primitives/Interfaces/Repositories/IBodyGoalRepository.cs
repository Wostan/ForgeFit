using ForgeFit.Domain.Aggregates.GoalAggregate;

namespace ForgeFit.Domain.Primitives.Interfaces.Repositories;

public interface IBodyGoalRepository : IRepository<BodyGoal>
{
    Task<BodyGoal?> GetByUserIdAsync(Guid userId);
}