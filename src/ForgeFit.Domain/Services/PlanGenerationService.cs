using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Primitives.Interfaces;
using ForgeFit.Domain.ValueObjects.GoalValueObjects;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Domain.Services;

public class PlanGenerationService(
    IWorkoutPlanGenerationService workoutPlanGenerationService,
    IDailyNutritionPlanCalculationService dailyNutritionPlanCalculationService
) : IPlanGenerationService
{
    public (WorkoutPlan workoutPlan, DailyNutritionPlan nutritionPlan)
        GenerateFullPlan(UserProfile userProfile, BodyGoal bodyGoal)
    {
        var workoutPlan = workoutPlanGenerationService.GenerateWorkoutPlan(userProfile, bodyGoal);
        var nutritionPlan =
            dailyNutritionPlanCalculationService.CalculateDailyNutritionPlan(userProfile, bodyGoal, workoutPlan);

        return (workoutPlan, nutritionPlan);
    }
}
