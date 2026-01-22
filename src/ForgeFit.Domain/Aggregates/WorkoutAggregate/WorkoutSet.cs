using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Domain.Aggregates.WorkoutAggregate;

public class WorkoutSet : Entity
{
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
    }

    public Guid UserId { get; private set; }
    public Guid WorkoutExercisePlanId { get; private set; }
    public int Order { get; private set; }
    public int Reps { get; private set; }
    public TimeSpan RestTime { get; private set; }
    public Weight Weight { get; private set; }

    public WorkoutExercisePlan WorkoutExercisePlan { get; private set; }

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

    public void Update(int order, int reps, TimeSpan restTime, Weight weight)
    {
        SetOrder(order);
        SetReps(reps);
        SetRestTime(restTime);
        SetWeight(weight);
    }
}
