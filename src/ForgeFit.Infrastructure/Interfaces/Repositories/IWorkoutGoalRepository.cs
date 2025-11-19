using ForgeFit.Domain.Aggregates.GoalAggregate;

namespace ForgeFit.Infrastructure.Interfaces.Repositories;

public interface IWorkoutGoalRepository : IRepository<WorkoutGoal>
{
    Task<WorkoutGoal?> GetByUserIdAsync(Guid userId);
}