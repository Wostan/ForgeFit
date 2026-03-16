using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Enums.WorkoutEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.GoalValueObjects;

public class WorkoutPlan : ValueObject
{
    #region Constructors
    public WorkoutPlan(
        int workoutsPerWeek,
        TimeSpan duration,
        WorkoutType workoutType)
    {
        SetWorkoutsPerWeek(workoutsPerWeek);
        SetDuration(duration);
        SetWorkoutType(workoutType);
    }
    #endregion

    #region Public Properties
    public int WorkoutsPerWeek { get; private set; }
    public TimeSpan Duration { get; private set; }
    public WorkoutType WorkoutType { get; private set; }
    #endregion

    #region Private Methods
    private void SetWorkoutsPerWeek(int workoutsPerWeek)
    {
        if (workoutsPerWeek is < DomainConstants.ValidationLimits.MinWorkoutsPerWeek or > DomainConstants.ValidationLimits.MaxWorkoutsPerWeek)
            throw new DomainValidationException($"WorkoutsPerWeek must be between {DomainConstants.ValidationLimits.MinWorkoutsPerWeek} and {DomainConstants.ValidationLimits.MaxWorkoutsPerWeek}.");

        WorkoutsPerWeek = workoutsPerWeek;
    }

    private void SetDuration(TimeSpan recommendedDuration)
    {
        if (recommendedDuration < TimeSpan.FromMinutes(DomainConstants.ValidationLimits.MinWorkoutDurationMinutes) || 
            recommendedDuration > TimeSpan.FromHours(DomainConstants.ValidationLimits.MaxWorkoutDurationHours))
            throw new DomainValidationException($"Workout duration must be between {DomainConstants.ValidationLimits.MinWorkoutDurationMinutes} minutes and {DomainConstants.ValidationLimits.MaxWorkoutDurationHours} hours.");

        Duration = recommendedDuration;
    }

    private void SetWorkoutType(WorkoutType recommendedWorkoutType)
    {
        if (!Enum.IsDefined(recommendedWorkoutType))
            throw new DomainValidationException("WorkoutType is not defined.");

        WorkoutType = recommendedWorkoutType;
    }
    #endregion

    #region ValueObject Implementation
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return WorkoutsPerWeek;
        yield return Duration;
        yield return WorkoutType;
    }
    #endregion
}
