using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Enums.WorkoutEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Domain.Aggregates.GoalAggregate;

public class WorkoutGoal : Entity, ITimeFields
{
    internal WorkoutGoal(Guid userId,
        int workoutsPerWeek,
        Schedule recommendedSchedule,
        WorkoutType recommendedWorkoutType)
    {
        SetUserId(userId);
        SetWorkoutsPerWeek(workoutsPerWeek);
        SetRecommendedSchedule(recommendedSchedule);
        SetRecommendedWorkoutType(recommendedWorkoutType);
        CreatedAt = DateTime.UtcNow;
    }

    private WorkoutGoal()
    {
    }
    
    public Guid UserId { get; private set; }
    public int WorkoutsPerWeek { get; private set; }
    public Schedule RecommendedSchedule { get; private set; }
    public WorkoutType RecommendedWorkoutType { get; private set; }
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
    
    private void SetRecommendedSchedule(Schedule recommendedSchedule)
    {
        var duration = recommendedSchedule.Duration;

        if (duration < TimeSpan.FromMinutes(10) || duration > TimeSpan.FromHours(5))
            throw new DomainValidationException("Workout duration hours must be between 10 minutes and 5 hours.");
        
        RecommendedSchedule = recommendedSchedule;
    }
    
    private void SetRecommendedWorkoutType(WorkoutType recommendedWorkoutType)
    {
        if (!Enum.IsDefined(recommendedWorkoutType))
            throw new DomainValidationException("RecommendedWorkoutType is not defined.");
        
        RecommendedWorkoutType = recommendedWorkoutType;
    }
    
    public void UpdateWorkoutGoal(
        int workoutsPerWeek,
        Schedule recommendedSchedule,
        WorkoutType recommendedWorkoutType)
    {
        SetWorkoutsPerWeek(workoutsPerWeek);
        SetRecommendedSchedule(recommendedSchedule);
        SetRecommendedWorkoutType(recommendedWorkoutType);
        UpdatedAt = DateTime.UtcNow;
    }
}