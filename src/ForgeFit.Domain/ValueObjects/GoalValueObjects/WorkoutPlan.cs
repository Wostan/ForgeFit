using ForgeFit.Domain.Enums.WorkoutEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.GoalValueObjects;

public class WorkoutPlan : ValueObject
{
    public WorkoutPlan(
        int workoutsPerWeek,
        TimeSpan duration, 
        WorkoutType workoutType)
    {
        SetWorkoutsPerWeek(workoutsPerWeek);
        SetDuration(duration);
        SetWorkoutType(workoutType);
    }
    
    public int WorkoutsPerWeek { get; private set; }
    public TimeSpan Duration { get; private set; }
    public WorkoutType WorkoutType { get; private set; }
    
    private void SetWorkoutsPerWeek(int workoutsPerWeek)
    {
        if (workoutsPerWeek is < 1 or > 7)
            throw new DomainValidationException("WorkoutsPerWeek must be between 1 and 7.");

        WorkoutsPerWeek = workoutsPerWeek;
    }
    
    private void SetDuration(TimeSpan recommendedDuration)
    {
        if (recommendedDuration < TimeSpan.FromMinutes(10) || recommendedDuration > TimeSpan.FromHours(5))
            throw new DomainValidationException("Workout duration hours must be between 10 minutes and 5 hours.");
        
        Duration = recommendedDuration;
    }
    
    private void SetWorkoutType(WorkoutType recommendedWorkoutType)
    {
        if (!Enum.IsDefined(recommendedWorkoutType))
            throw new DomainValidationException("WorkoutType is not defined.");
        
        WorkoutType = recommendedWorkoutType;
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return WorkoutsPerWeek;
        yield return Duration;
        yield return WorkoutType;
    }
}