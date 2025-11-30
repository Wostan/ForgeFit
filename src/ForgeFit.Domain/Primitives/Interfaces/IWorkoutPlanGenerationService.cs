using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.ValueObjects.GoalValueObjects;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Domain.Primitives.Interfaces;

public interface IWorkoutPlanGenerationService
{
    WorkoutPlan GenerateWorkoutPlan(UserProfile userProfile, BodyGoal bodyGoal);
}