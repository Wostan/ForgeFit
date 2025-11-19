using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Aggregates.UserAggregate;

namespace ForgeFit.Domain.Primitives.Interfaces;

public interface IWorkoutGoalGenerationService
{
    WorkoutGoal GenerateWorkoutGoal(User user, BodyGoal bodyGoal);
}