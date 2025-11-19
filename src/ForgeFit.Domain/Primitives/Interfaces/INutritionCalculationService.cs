using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Aggregates.UserAggregate;

namespace ForgeFit.Domain.Primitives.Interfaces;

public interface INutritionCalculationService
{
    NutritionGoal CalculateNutritionGoal(User user, BodyGoal bodyGoal, WorkoutGoal? workoutGoal);
}