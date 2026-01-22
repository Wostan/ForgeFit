using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Enums.GoalEnums;
using ForgeFit.Domain.Enums.WorkoutEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives.Interfaces;
using ForgeFit.Domain.ValueObjects.GoalValueObjects;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Domain.Services;

public class WorkoutPlanGenerationService : IWorkoutPlanGenerationService
{
    private const double MaintenanceRate = 0.15;
    private const double ModerateRate = 0.35;

    public WorkoutPlan GenerateWorkoutPlan(UserProfile userProfile, BodyGoal bodyGoal)
    {
        var weightKg = userProfile.Weight.ToKg().Value;
        var goalWeightKg = bodyGoal.WeightGoal.ToKg().Value;
        var age = userProfile.DateOfBirth.GetAge();

        ValidateGoalRealism(weightKg, goalWeightKg, bodyGoal.DueDate, bodyGoal.GoalType);

        var weeksToGoal = CalculateWeeksToGoal(bodyGoal.DueDate);

        var weightChangePerWeek = CalculateWeightChangePerWeek(
            weightKg,
            goalWeightKg,
            weeksToGoal);

        var workoutsPerWeek = CalculateWorkoutsPerWeek(age,
            weightChangePerWeek);

        var recommendedDuration = CalculateRecommendedDuration(
            bodyGoal.GoalType,
            age,
            weightChangePerWeek);

        var workoutType = SetWorkoutType(bodyGoal);

        return new WorkoutPlan(workoutsPerWeek, recommendedDuration, workoutType);
    }

    private static void ValidateGoalRealism(double current, double target, DateTime? date, GoalType type)
    {
        if (!date.HasValue) return;
        var days = (date.Value - DateTime.UtcNow).TotalDays;
        if (days < 7) throw new UnrealisticGoalException("Deadline is too close (min 7 days).");

        var weeks = days / 7.0;
        var rate = Math.Abs(target - current) / weeks;

        switch (type)
        {
            case GoalType.FatLoss when rate > 1.3:
                throw new UnrealisticGoalException($"Losing {rate:F1}kg/week is unsafe.");
            case GoalType.MuscleGain when rate > 0.6:
                throw new UnrealisticGoalException($"Gaining {rate:F1}kg/week muscle is unrealistic.");
        }
    }

    private static int? CalculateWeeksToGoal(DateTime? dueDate)
    {
        if (!dueDate.HasValue) return null;

        var days = (dueDate.Value.Date - DateTime.UtcNow.Date).TotalDays;
        return days > 0 ? (int)Math.Ceiling(days / 7.0) : null;
    }

    private static double? CalculateWeightChangePerWeek(double current, double target, int? weeks)
    {
        if (weeks is null or <= 0) return null;
        return Math.Abs(target - current) / weeks;
    }

    private static int CalculateWorkoutsPerWeek(int age, double? ratePerWeek)
    {
        var rate = ratePerWeek ?? 0.25;

        var frequency = rate switch
        {
            <= MaintenanceRate => 2,
            <= ModerateRate => 3,
            _ => 4
        };

        switch (age)
        {
            case < 25 when rate > ModerateRate:
                frequency += 1;
                break;
            case > 50:
                frequency -= 1;
                break;
        }

        return Math.Clamp(frequency, 2, 5);
    }

    private static TimeSpan CalculateRecommendedDuration(
        GoalType goalType,
        int age,
        double? ratePerWeek)
    {
        var rate = ratePerWeek ?? 0.25;

        var minutes = 60;

        if (goalType == GoalType.MuscleGain)
            minutes = 75;

        if (rate <= MaintenanceRate)
            minutes -= 15;

        if (goalType == GoalType.FatLoss && rate > ModerateRate)
            minutes += 15;

        if (age > 50)
            minutes -= 10;

        return TimeSpan.FromMinutes(Math.Clamp(minutes, 30, 120));
    }

    private static WorkoutType SetWorkoutType(BodyGoal bodyGoal)
    {
        return bodyGoal.GoalType switch
        {
            GoalType.MuscleGain => WorkoutType.Hypertrophy,
            GoalType.WeightGain => WorkoutType.StrengthTraining,
            _ => WorkoutType.StrengthCardio
        };
    }
}
