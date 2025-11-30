using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.ValueObjects.GoalValueObjects;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Domain.Primitives.Interfaces;

public interface IPlanGenerationService
{
    (WorkoutPlan workoutPlan, DailyNutritionPlan nutritionPlan) 
        GenerateFullPlan(UserProfile userProfile, BodyGoal bodyGoal);
}