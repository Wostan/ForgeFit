using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Enums.GoalEnums;
using ForgeFit.Domain.Enums.ProfileEnums;
using ForgeFit.Domain.Enums.WorkoutEnums;
using ForgeFit.Domain.Primitives.Interfaces;
using ForgeFit.Domain.ValueObjects.GoalValueObjects;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Domain.Services;

public class DailyNutritionPlanCalculationService : IDailyNutritionPlanCalculationService
{
    private const int CaloriesPerKg = 7700;
    
    public DailyNutritionPlan CalculateDailyNutritionPlan(UserProfile userProfile, BodyGoal bodyGoal, WorkoutPlan? workoutPlan)
    {
        var weightKg = userProfile.Weight.ToKg().Value;
        var heightCm = userProfile.Height.ToCm().Value;
        var age = userProfile.DateOfBirth.GetAge();
        var gender = userProfile.Gender;
        
        var bmr = CalculateBmr(weightKg, heightCm, age, gender);
        
        var activityFactor = CalculateActivityFactor(workoutPlan);
        var tdee = bmr * activityFactor;
        
        var targetCalories = CalculateCaloriesForGoal(tdee, weightKg, bodyGoal);
        
        var macros = CalculateMacronutrients(targetCalories, weightKg, bodyGoal);
        
        var waterGoalMl = CalculateWaterIntake(weightKg, workoutPlan);

        return new DailyNutritionPlan(
            targetCalories,
            macros.Carbs,
            macros.Protein,
            macros.Fat,
            waterGoalMl);
    }

    private static double CalculateBmr(double weight, double height, int age, Gender gender)
    {
        var bmr = 10 * weight + 6.25 * height - 5 * age;
        return gender == Gender.Male ? bmr + 5 : bmr - 161;
    }
    
    private static double CalculateActivityFactor(WorkoutPlan? workoutPlan)
    {
        if (workoutPlan == null)
            return 1.2;
        
        var workoutsPerWeek = workoutPlan.WorkoutsPerWeek;
        var duration = workoutPlan.Duration;
        
        var baseFactor = workoutsPerWeek switch
        {
            1 => 1.25,
            2 => 1.35,
            3 => 1.45,
            4 => 1.55,
            5 => 1.65,
            _ => 1.75
        };
        
        var durationMultiplier = duration switch
        {
            _ when duration <= TimeSpan.FromMinutes(30) => 0.98,
            _ when duration <= TimeSpan.FromMinutes(60) => 1.0,
            _ when duration <= TimeSpan.FromMinutes(90) => 1.02,
            _ => 1.05
        };
        
        var factor = baseFactor * durationMultiplier;

        return factor;
    }
    
    private static int CalculateCaloriesForGoal(double tdee, double currentWeight, BodyGoal bodyGoal)
    {
        if (!bodyGoal.DueDate.HasValue)
        {
            return bodyGoal.GoalType switch
            {
                GoalType.FatLoss => (int)(tdee * 0.85),
                GoalType.MuscleGain => (int)(tdee + 200),
                GoalType.WeightGain => (int)(tdee + 300),
                _ => (int)tdee
            };
        }

        var targetWeight = bodyGoal.WeightGoal.ToKg().Value;
        var totalWeightDiff = targetWeight - currentWeight;
        
        var daysToGoal = (bodyGoal.DueDate.Value - DateTime.UtcNow).TotalDays;
        if (daysToGoal < 7) 
            daysToGoal = 7; 

        var dailyCalorieDiff = (totalWeightDiff * CaloriesPerKg) / daysToGoal;

        
        dailyCalorieDiff = Math.Clamp(dailyCalorieDiff, -1000, 1000);

        if (bodyGoal.GoalType == GoalType.MuscleGain && dailyCalorieDiff < 150)
            dailyCalorieDiff = 150;
        
        var target = tdee + dailyCalorieDiff;

        if (bodyGoal.GoalType != GoalType.FatLoss) 
            return (int)target;
        
        var minSafeCalories = Math.Max(1200, tdee * 0.6);
        if (target < minSafeCalories) 
            target = minSafeCalories;

        return (int)target;
    }
    
    private static (int Carbs, int Protein, int Fat) CalculateMacronutrients(
        int targetCalories,
        double weightKg,
        BodyGoal bodyGoal)
    {
        var proteinPerKg = bodyGoal.GoalType switch
        {
            GoalType.MuscleGain => 2.0, 
            GoalType.FatLoss => 2.1,
            GoalType.WeightGain => 1.8, 
            _ => 1.6
        };
        var proteinGrams = (int)(weightKg * proteinPerKg);
        
        var fatPerKg = bodyGoal.GoalType == GoalType.WeightGain ? 1.2 : 1.0;
        var fatGrams = (int)(weightKg * fatPerKg);

        var takenCalories = proteinGrams * 4 + fatGrams * 9;
        var remainingCalories = targetCalories - takenCalories;
        
        if (remainingCalories < 0)
        {
            fatGrams = (int)(weightKg * 0.8);
            remainingCalories = targetCalories - (proteinGrams * 4 + fatGrams * 9);
            if (remainingCalories < 0) remainingCalories = 0;
        }

        var carbsGrams = remainingCalories / 4;
        
        if (carbsGrams < 80) carbsGrams = 80;

        return (carbsGrams, proteinGrams, fatGrams);
    }
    
    private static int CalculateWaterIntake(double weightKg, WorkoutPlan? workoutPlan)
    {
        var baseWaterMl = weightKg * 35;

        if (workoutPlan == null)
            return (int)Math.Max(baseWaterMl, 1800);
        
        var durationHours = workoutPlan.Duration.TotalMinutes / 60.0;
        var sweatLoss = durationHours * 500; 
            
        var avgDailyExtra = sweatLoss * workoutPlan.WorkoutsPerWeek / 7.0;
        baseWaterMl += avgDailyExtra;

        return (int)Math.Max(baseWaterMl, 1800);
    }
}