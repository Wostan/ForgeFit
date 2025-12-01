using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Domain.Aggregates.GoalAggregate;

namespace ForgeFit.Application.Common.Interfaces.Repositories;

public interface INutritionGoalRepository : IRepository<NutritionGoal>
{
    Task<NutritionGoal?> GetByUserIdAsync(Guid userId);
}
