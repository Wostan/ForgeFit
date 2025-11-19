using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Enums.GoalEnums;
using ForgeFit.Domain.Enums.ProfileEnums;
using ForgeFit.Domain.Enums.WorkoutEnums;
using ForgeFit.Domain.Primitives.Interfaces;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Domain.Services;

public class NutritionCalculationService : INutritionCalculationService
{
    public NutritionGoal CalculateNutritionGoal(User user, BodyGoal bodyGoal, WorkoutGoal? workoutGoal)
    {
        var userProfile = user.UserProfile;
        var weightKg = userProfile.Weight.ToKg().Value;
        var heightCm = userProfile.Height.ToCm().Value;
        var age = CalculateAge(userProfile.DateOfBirth);
        var gender = userProfile.Gender;
        
        var bmr = CalculateBmr(weightKg, heightCm, age, gender);
        
        var activityFactor = CalculateActivityFactor(workoutGoal);
        
        var tdee = bmr * activityFactor;
        
        var targetCalories = CalculateCaloriesForGoal(tdee, bodyGoal, workoutGoal);
        
        var macros = CalculateMacronutrients(targetCalories, weightKg, bodyGoal, workoutGoal);
        
        var waterGoalMl = CalculateWaterIntake(weightKg, workoutGoal);
        
        return new NutritionGoal(
            user.Id,
            targetCalories,
            macros.Carbs,
            macros.Protein,
            macros.Fat,
            waterGoalMl
        );
    }

    private static int CalculateAge(DateOfBirth dob)
    {
        var dateOfBirth = dob.Value;
        var today = DateTime.UtcNow;
        
        var age = today.Year - dateOfBirth.Year;
        
        if (dateOfBirth.Date > today.AddYears(-age))
            age--;
        
        return age;
    }

    private static double CalculateBmr(double weight, double height, int age, Gender gender)
    {
        var bmr = 10 * weight + 6.25 * height - 5 * age;
        return gender == Gender.Male ? bmr + 5 : bmr - 161;
    }
    
    private static double CalculateActivityFactor(WorkoutGoal? workoutGoal)
    {
        if (workoutGoal == null)
            return 1.2;
        
        const double minFactor = 1.1;
        const double maxFactor = 2.0;
        
        var workoutsPerWeek = workoutGoal.WorkoutsPerWeek;
        var workoutType = workoutGoal.WorkoutType;
        var duration = workoutGoal.Duration;
        
        var baseFactor = workoutsPerWeek switch
        {
            0 => 1.2,
            1 => 1.2,
            <= 3 => 1.375,
            <= 5 => 1.55,
            _ => 1.725
        };
        
        var workoutTypeMultiplier = workoutType switch
        {
            WorkoutType.StrengthCardio => 1.05,
            WorkoutType.StrengthTraining => 1.08,
            WorkoutType.Hypertrophy => 1.1,
            _ => 1.0
        };
        
        var durationMultiplier = duration switch
        {
            _ when duration <= TimeSpan.FromMinutes(30) => 0.9,
            _ when duration <= TimeSpan.FromMinutes(60) => 1.0,
            _ => 1.1
        };
        
        var factor = baseFactor * workoutTypeMultiplier * durationMultiplier;
        
        return Math.Round(Math.Clamp(factor, minFactor, maxFactor), 3);
    }
    
    private static int CalculateCaloriesForGoal(double tdee, BodyGoal bodyGoal, WorkoutGoal? workoutGoal)
    {
        return bodyGoal.GoalType switch
        {
            GoalType.FatLoss => (int)Math.Round(tdee - 500),
            GoalType.WeightGain => (int)Math.Round(tdee + 400),
            GoalType.MuscleGain => CalculateMuscleGainCalories(tdee, workoutGoal),
            _ => (int)Math.Round(tdee)
        };
    }
    
    private static int CalculateMuscleGainCalories(double tdee, WorkoutGoal? workoutGoal)
    {
        if (workoutGoal != null && workoutGoal.WorkoutsPerWeek >= 4)
            return (int)Math.Round(tdee + 500); 
        
        return (int)Math.Round(tdee + 300);
    }
    
    private static (int Protein, int Carbs, int Fat) CalculateMacronutrients(
        int targetCalories, 
        double weight, 
        BodyGoal bodyGoal, 
        WorkoutGoal? workoutGoal)
    {
        var protein = CalculateProtein(weight, bodyGoal, workoutGoal, targetCalories);
        
        var (carbs, fat) = CalculateFatsAndCarbs(targetCalories, protein, bodyGoal, workoutGoal);
        
        return (protein, carbs, fat);
    }
    
    private static int CalculateProtein(double weightKg, BodyGoal bodyGoal, WorkoutGoal? workoutGoal, int targetCalories)
    {
        double proteinGramsPerKg;
        
        if (bodyGoal.GoalType == GoalType.MuscleGain || 
            (workoutGoal != null && workoutGoal.WorkoutType == WorkoutType.Hypertrophy))
        {
            proteinGramsPerKg = 2.0;
        }
        else if (bodyGoal.GoalType == GoalType.FatLoss)
        {
            proteinGramsPerKg = 2.2;
        }
        else if (workoutGoal != null && workoutGoal.WorkoutType == WorkoutType.StrengthTraining)
        {
            proteinGramsPerKg = 1.8;
        }
        else
        {
            proteinGramsPerKg = 1.4;
        }
        
        var totalProteinGrams = weightKg * proteinGramsPerKg;
        
        var maxProteinFromCalories = (int)(targetCalories * 0.35 / 4);
        
        return (int)Math.Min(Math.Round(totalProteinGrams), maxProteinFromCalories);
    }
    
    private static (int Carbs, int Fats) CalculateFatsAndCarbs(
        int targetCalories, 
        int proteinGrams, 
        BodyGoal bodyGoal,
        WorkoutGoal? workoutGoal)
    {
        var proteinCalories = proteinGrams * 4;
        var remainingCalories = targetCalories - proteinCalories;
        
        if (remainingCalories < 0) remainingCalories = 0;

        double carbRatio;
        
        if (bodyGoal.GoalType == GoalType.MuscleGain || 
            (workoutGoal != null && workoutGoal.WorkoutType == WorkoutType.Hypertrophy))
        {
            carbRatio = 0.55 / (0.55 + 0.25);
        }
        else if (workoutGoal != null && workoutGoal.WorkoutType == WorkoutType.StrengthCardio)
        {
            carbRatio = 0.50 / (0.50 + 0.28);
        }
        else if (bodyGoal.GoalType == GoalType.FatLoss)
        {
            carbRatio = 0.40 / (0.40 + 0.30);
        }
        else
        {
            carbRatio = 0.48 / (0.48 + 0.27);
        }
        
        var carbCalories = (int)Math.Round(remainingCalories * carbRatio);
        var fatCalories = remainingCalories - carbCalories; 
        
        var carbGrams = carbCalories / 4;
        var fatGrams = fatCalories / 9;

        return (carbGrams, fatGrams);
    }
    
    private static int CalculateWaterIntake(double weightKg, WorkoutGoal? workoutGoal)
    {
        var baseWaterMl = weightKg * 35;
        
        if (workoutGoal == null) 
            return (int)Math.Round(baseWaterMl);
        
        var workoutDurationMinutes = workoutGoal.Duration.TotalMinutes;
        var additionalWater = workoutDurationMinutes / 30.0 * 350;
        
        var avgDailyExtraWater = additionalWater * workoutGoal.WorkoutsPerWeek / 7.0;
            
        baseWaterMl += avgDailyExtraWater;

        return (int)Math.Round(baseWaterMl);
    }
}