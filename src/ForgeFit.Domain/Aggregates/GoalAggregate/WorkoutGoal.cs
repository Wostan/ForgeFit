using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Enums.WorkoutEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects.GoalValueObjects;

namespace ForgeFit.Domain.Aggregates.GoalAggregate;

public class WorkoutGoal : Entity, ITimeFields
{
    internal WorkoutGoal(Guid userId, WorkoutPlan workoutPlan)
    {
        SetUserId(userId);
        SetWorkoutPlan(workoutPlan);
        CreatedAt = DateTime.UtcNow;
    }

    private WorkoutGoal()
    {
    }
    
    public Guid UserId { get; private set; }
    public WorkoutPlan WorkoutPlan { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public User User { get; private set; }
    
    public static WorkoutGoal Create(Guid userId, WorkoutPlan workoutPlan)
    {
        return new WorkoutGoal(userId, workoutPlan);
    }
    
    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException("UserId cannot be empty.");

        UserId = userId;
    }
    
    private void SetWorkoutPlan(WorkoutPlan workoutPlan)
    {
        WorkoutPlan = workoutPlan ?? throw new DomainValidationException("WorkoutPlan cannot be null.");
    }
    
    public void Update(
        int workoutsPerWeek,
        TimeSpan duration, 
        WorkoutType workoutType)
    {
        SetWorkoutPlan(new WorkoutPlan(workoutsPerWeek, duration, workoutType));
        UpdatedAt = DateTime.UtcNow;
    }
}