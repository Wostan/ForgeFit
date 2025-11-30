using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Domain.Aggregates.WorkoutAggregate;

public class WorkoutSet : Entity
{
    internal WorkoutSet(
        Guid userId,
        int order,
        int reps,
        TimeSpan restTime,
        Weight weight)
    {
        SetUserId(userId);
        SetOrder(order);
        SetReps(reps);
        SetRestTime(restTime);
        SetWeight(weight);
    }
    
    private  WorkoutSet()
    {
    }
    
    public Guid UserId { get; private set; }
    public int Order { get; private set; }
    public int Reps { get; private set; }
    public TimeSpan RestTime { get; private set; }
    public Weight Weight { get; private set; }
    
    // Navigation properties
    public WorkoutExercisePlan WorkoutExercisePlan { get; private set; }
    
    public static WorkoutSet Create(
        Guid userId,
        int order,
        int reps,
        TimeSpan restTime,
        Weight weight)
    {
        return new WorkoutSet(userId, order, reps, restTime, weight);
    }
    
    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException("UserId cannot be empty.");

        UserId = userId;
    }
    
    private void SetOrder(int order)
    {
        if (order < 1)
            throw new DomainValidationException("Order must be greater than 0.");

        Order = order;
    }
    
    private void SetReps(int reps)
    {
        if (reps < 1)
            throw new DomainValidationException("Reps must be greater than 0.");

        Reps = reps;
    }
    
    private void SetRestTime(TimeSpan restTime)
    {
        if (restTime < TimeSpan.Zero)
            throw new DomainValidationException("Rest time must be greater than 0.");

        RestTime = restTime;
    }
    
    private void SetWeight(Weight weight)
    {
        Weight = weight ?? throw new DomainValidationException("Weight cannot be null.");
    }
    
    public void Update(int order, int reps, TimeSpan restTime, Weight weight)
    {
        SetOrder(order);
        SetReps(reps);
        SetRestTime(restTime);
        SetWeight(weight);
    }
}