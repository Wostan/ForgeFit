using ForgeFit.Domain.Aggregates.GoalAggregate;

namespace ForgeFit.Application.Common.Interfaces.Repositories;

public interface IWorkoutGoalRepository : IRepository<WorkoutGoal>
{
    Task<WorkoutGoal?> GetByUserIdAsync(Guid userId);
}