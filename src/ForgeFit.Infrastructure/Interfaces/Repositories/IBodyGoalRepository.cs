using ForgeFit.Domain.Aggregates.GoalAggregate;

namespace ForgeFit.Infrastructure.Interfaces.Repositories;

public interface IBodyGoalRepository : IRepository<BodyGoal>
{
    Task<BodyGoal?> GetByUserIdAsync(Guid userId);
}