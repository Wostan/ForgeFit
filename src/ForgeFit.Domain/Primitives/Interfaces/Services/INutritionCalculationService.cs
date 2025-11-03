using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Domain.Primitives.Interfaces.Services;

public interface INutritionCalculationService
{
    NutritionGoal CalculateNutritionGoal(UserProfile userProfile, BodyGoal bodyGoal, WorkoutGoal? workoutGoal);
}