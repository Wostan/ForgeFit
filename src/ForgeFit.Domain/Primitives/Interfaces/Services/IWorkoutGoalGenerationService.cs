using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Domain.Primitives.Interfaces.Services;

public interface IWorkoutGoalGenerationService
{
    WorkoutGoal GenerateWorkoutGoal(UserProfile userProfile, BodyGoal bodyGoal);
}