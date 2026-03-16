using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Enums.WorkoutEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects.GoalValueObjects;

namespace ForgeFit.Domain.Aggregates.GoalAggregate;

public class WorkoutGoal : Entity, ITimeFields
{
    #region Private Fields
    #endregion

    #region Constructors
    internal WorkoutGoal(Guid userId, WorkoutPlan workoutPlan)
    {
        SetUserId(userId);
        SetWorkoutPlan(workoutPlan);
        CreatedAt = DateTime.UtcNow;
    }

    private WorkoutGoal()
    {
    }
    #endregion

    #region Public Properties
    public Guid UserId { get; private set; }
    public WorkoutPlan WorkoutPlan { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    #endregion

    #region Navigation Properties
    public User User { get; private set; }
    #endregion

    #region Factory Methods
    public static WorkoutGoal Create(Guid userId, WorkoutPlan workoutPlan)
    {
        return new WorkoutGoal(userId, workoutPlan);
    }
    #endregion

    #region Domain Methods
    public void Update(
        int workoutsPerWeek,
        TimeSpan duration,
        WorkoutType workoutType)
    {
        SetWorkoutPlan(new WorkoutPlan(workoutsPerWeek, duration, workoutType));
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion

    #region Private Setters
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
    #endregion
}
