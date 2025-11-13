using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Enums.WorkoutEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Aggregates.GoalAggregate;

public class WorkoutGoal : Entity, ITimeFields
{
    internal WorkoutGoal(Guid userId,
        int workoutsPerWeek,
        TimeSpan duration, 
        WorkoutType workoutType)
    {
        SetUserId(userId);
        SetWorkoutsPerWeek(workoutsPerWeek);
        SetDuration(duration);
        SetWorkoutType(workoutType);
        CreatedAt = DateTime.UtcNow;
    }

    private WorkoutGoal()
    {
    }
    
    public Guid UserId { get; private set; }
    public int WorkoutsPerWeek { get; private set; }
    public TimeSpan Duration { get; private set; }
    public WorkoutType WorkoutType { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public User User { get; private set; }
    
    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException("UserId cannot be empty.");

        UserId = userId;
    }
    
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
    
    public void UpdateWorkoutGoal(
        int workoutsPerWeek,
        TimeSpan duration,
        WorkoutType recommendedWorkoutType)
    {
        SetWorkoutsPerWeek(workoutsPerWeek);
        SetDuration(duration);
        SetWorkoutType(recommendedWorkoutType);
        UpdatedAt = DateTime.UtcNow;
    }
}