using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Enums.GoalEnums;
using ForgeFit.Domain.Enums.WorkoutEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives.Interfaces;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Domain.Services;

public class WorkoutGoalGenerationService : IWorkoutGoalGenerationService
{
    private const double SlowMuscleGainWeekly = 0.125;
    private const double AggressiveMuscleGainWeekly = 0.375;
    private const double SlowFatLossWeekly = 0.5;
    private const double AggressiveFatLossWeekly = 1.0;
    private const double SlowWeightGainWeekly = 0.25;
    private const double AggressiveWeightGainWeekly = 0.75;
    
    public WorkoutGoal GenerateWorkoutGoal(User user, BodyGoal bodyGoal)
    {
        var userProfile = user.UserProfile;
        var weightKg = userProfile.Weight.ToKg().Value;
        var goalWeightKg = bodyGoal.WeightGoal.ToKg().Value;
        var age = CalculateAge(userProfile.DateOfBirth);
        
        var weeksToGoal = CalculateWeeksToGoal(bodyGoal.DueDate);
        
        var weightChangePerWeek = CalculateWeightChangePerWeek(weightKg, goalWeightKg, weeksToGoal);
        
        var workoutsPerWeek = CalculateWorkoutsPerWeek(bodyGoal, age, weightChangePerWeek, weeksToGoal);
        var recommendedDuration = CalculateRecommendedDuration(bodyGoal, age, weightChangePerWeek, weeksToGoal);
        var workoutType = SetWorkoutType(bodyGoal);
        
        return new WorkoutGoal(user.Id, workoutsPerWeek, recommendedDuration, workoutType);
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

    private static int? CalculateWeeksToGoal(DateTime? dueDate)
    {
        if (!dueDate.HasValue) 
            return null;
        
        var daysToGoal = (dueDate.Value.Date - DateTime.UtcNow.Date).TotalDays;
        if (daysToGoal <= 0) 
            return null;
        
        return (int)Math.Ceiling(daysToGoal / 7.0);
    }

    private static double? CalculateWeightChangePerWeek(
        double currentWeight, 
        double goalWeight, 
        int? weeksToGoal)
    {
        if (weeksToGoal is not > 0)
            return null;
            
        var totalWeightChange = goalWeight - currentWeight;
        return totalWeightChange / weeksToGoal.Value;
    }
    
    private static int CalculateWorkoutsPerWeek(
        BodyGoal bodyGoal,
        int age,
        double? weightChangePerWeek,
        int? weeksToGoal)
    {
        var baseFrequency = bodyGoal.GoalType switch
        {
            GoalType.MuscleGain => 4,
            GoalType.FatLoss => 4,
            GoalType.WeightGain => 3,
            _ => throw new DomainValidationException("GoalType is not defined.")
        };

        var ageModifier = age switch
        {
            < 25 => 1,
            < 40 => 0,
            < 55 => -1,
            _ => -2
        };
        baseFrequency = Math.Clamp(baseFrequency + ageModifier, 2, 6);

        if (weightChangePerWeek == null || weeksToGoal == null)
            return baseFrequency;
        
        var adjustedFrequency = baseFrequency;
        
        switch (bodyGoal.GoalType)
        {
            case GoalType.MuscleGain:
            {
                var weeklyGain = weightChangePerWeek.Value;
                adjustedFrequency = weeklyGain switch
                {
                    <= SlowMuscleGainWeekly => Math.Max(baseFrequency - 1, 3), 
                    > AggressiveMuscleGainWeekly => Math.Min(baseFrequency + 1, 6),
                    _ => adjustedFrequency
                };
                break;
            }
            case GoalType.FatLoss:
            {
                var weeklyLoss = Math.Abs(weightChangePerWeek.Value);
                adjustedFrequency = weeklyLoss switch
                {
                    <= SlowFatLossWeekly => Math.Max(baseFrequency - 1, 3),
                    > AggressiveFatLossWeekly => Math.Min(baseFrequency + 1, 6),
                    _ => adjustedFrequency
                };
                break;
            }
            case GoalType.WeightGain:
            {
                var weeklyGain = weightChangePerWeek.Value;
                adjustedFrequency = weeklyGain switch
                {
                    <= SlowWeightGainWeekly => Math.Max(baseFrequency - 1, 2),
                    > AggressiveWeightGainWeekly => Math.Min(baseFrequency + 1, 5),
                    _ => adjustedFrequency
                };
                break;
            }
            default:
                throw new DomainValidationException("GoalType is not defined.");
        }
        
        return Math.Clamp(adjustedFrequency, 2, 6);
    }
    
    private static TimeSpan CalculateRecommendedDuration(
        BodyGoal bodyGoal, 
        int age,
        double? weightChangePerWeek, 
        int? weeksToGoal)
    {
        var baseDuration = bodyGoal.GoalType switch
        {
            GoalType.MuscleGain => 75,
            GoalType.FatLoss => 60,
            GoalType.WeightGain => 75,
            _ => 60
        };

        var ageModifier = age switch
        {
            < 25 => 0,
            < 40 => 0,
            < 55 => -10,
            _ => -20
        };
        baseDuration = Math.Clamp(baseDuration + ageModifier, 45, 180);
        
        if (weightChangePerWeek == null || weeksToGoal == null)
            return TimeSpan.FromMinutes(baseDuration);
        
        var adjustedDuration = baseDuration;
        
        switch (bodyGoal.GoalType)
        {
            case GoalType.MuscleGain:
            {
                var weeklyGain = weightChangePerWeek.Value;
                adjustedDuration = weeklyGain switch
                {
                    <= SlowMuscleGainWeekly => Math.Min(baseDuration + 15, 120),
                    > AggressiveMuscleGainWeekly => Math.Min(baseDuration + 10, 120),
                    _ => adjustedDuration
                };
                break;
            }
            case GoalType.FatLoss:
            {
                var weeklyLoss = Math.Abs(weightChangePerWeek.Value);
                adjustedDuration = weeklyLoss switch
                {
                    <= SlowFatLossWeekly => baseDuration,
                    > AggressiveFatLossWeekly => Math.Min(baseDuration + 15, 90),
                    _ => adjustedDuration
                };
                break;
            }
            case GoalType.WeightGain:
            {
                var weeklyGain = weightChangePerWeek.Value;
            
                if (weeklyGain <= SlowWeightGainWeekly)
                    adjustedDuration = Math.Min(baseDuration + 15, 120);
                break;
            }
            default:
                throw new DomainValidationException("GoalType is not defined.");
        }
        
        return TimeSpan.FromMinutes(adjustedDuration);
    }
    
    private static WorkoutType SetWorkoutType(BodyGoal bodyGoal)
    {
        return bodyGoal.GoalType switch
        {
            GoalType.MuscleGain => WorkoutType.Hypertrophy,
            GoalType.WeightGain => WorkoutType.StrengthTraining,
            GoalType.FatLoss => WorkoutType.StrengthCardio,
            _ => throw new DomainValidationException("GoalType is not defined.")
        };
    }
}