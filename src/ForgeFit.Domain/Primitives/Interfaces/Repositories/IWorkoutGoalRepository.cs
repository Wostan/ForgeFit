using ForgeFit.Domain.Aggregates.GoalAggregate;

namespace ForgeFit.Domain.Primitives.Interfaces.Repositories;

public interface IWorkoutGoalRepository : IRepository<WorkoutGoal>
{
    Task<WorkoutGoal?> GetByUserIdAsync(Guid userId);
}