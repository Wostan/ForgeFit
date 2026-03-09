using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Domain.Aggregates.WorkoutAggregate;

public class WorkoutSet : Entity, ITimeFields
{
    #region Private Fields
    #endregion

    #region Constructors
    private WorkoutSet()
    {
    }

    internal WorkoutSet(
        Guid userId,
        Guid workoutExercisePlanId,
        int order,
        int reps,
        TimeSpan restTime,
        Weight weight
    )
    {
        Id = Guid.NewGuid();
        SetUserId(userId);
        SetWorkoutExercisePlanId(workoutExercisePlanId);
        SetOrder(order);
        SetReps(reps);
        SetRestTime(restTime);
        SetWeight(weight);
        CreatedAt = DateTime.UtcNow;
    }
    #endregion

    #region Public Properties
    public Guid UserId { get; private set; }
    public Guid WorkoutExercisePlanId { get; private set; }
    public int Order { get; private set; }
    public int Reps { get; private set; }
    public TimeSpan RestTime { get; private set; }
    public Weight Weight { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    #endregion

    #region Navigation Properties
    public WorkoutExercisePlan WorkoutExercisePlan { get; private set; }
    #endregion

    #region Factory Methods
    public static WorkoutSet Create(
        Guid userId,
        Guid workoutExercisePlanId,
        int order,
        int reps,
        TimeSpan restTime,
        Weight weight
    )
    {
        return new WorkoutSet(userId, workoutExercisePlanId, order, reps, restTime, weight);
    }
    #endregion

    #region Domain Methods
    public void Update(int order, int reps, TimeSpan restTime, Weight weight)
    {
        SetOrder(order);
        SetReps(reps);
        SetRestTime(restTime);
        SetWeight(weight);
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion

    #region Private Setters
    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty) throw new DomainValidationException("UserId cannot be empty.");
        UserId = userId;
    }

    private void SetWorkoutExercisePlanId(Guid planId)
    {
        if (planId == Guid.Empty) throw new DomainValidationException("PlanId required");
        WorkoutExercisePlanId = planId;
    }

    private void SetOrder(int order)
    {
        if (order < 1) throw new DomainValidationException("Order must be greater than 0");
        Order = order;
    }

    private void SetReps(int reps)
    {
        if (reps < 1) throw new DomainValidationException("Reps must be greater than 0");
        Reps = reps;
    }

    private void SetRestTime(TimeSpan restTime)
    {
        if (restTime < TimeSpan.Zero) throw new DomainValidationException("Rest time cannot be negative");
        RestTime = restTime;
    }

    private void SetWeight(Weight weight)
    {
        Weight = weight ?? throw new DomainValidationException("Weight cannot be null");
    }
    #endregion
}
