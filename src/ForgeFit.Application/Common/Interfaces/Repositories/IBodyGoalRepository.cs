using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Domain.Aggregates.GoalAggregate;

namespace ForgeFit.Application.Common.Interfaces.Repositories;

public interface IBodyGoalRepository : IRepository<BodyGoal>
{
    Task<BodyGoal?> GetByUserIdAsync(Guid userId);
}
