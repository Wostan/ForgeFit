using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;
using ForgeFit.Domain.ValueObjects.WorkoutValueObjects;

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
        WorkoutSetInfo workoutSetInfo,
        RestTime restTime
    )
    {
        Id = Guid.NewGuid();
        SetUserId(userId);
        SetWorkoutExercisePlanId(workoutExercisePlanId);
        SetWorkoutSetInfo(workoutSetInfo);
        SetRestTime(restTime);
        CreatedAt = DateTime.UtcNow;
    }
    #endregion

    #region Public Properties
    public Guid UserId { get; private set; }
    public Guid WorkoutExercisePlanId { get; private set; }
    public WorkoutSetInfo WorkoutSetInfo { get; private set; }
    public RestTime RestTime { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    #endregion

    #region Navigation Properties
    public WorkoutExercisePlan WorkoutExercisePlan { get; private set; }
    #endregion

    #region Factory Methods
    public static WorkoutSet Create(
        Guid userId,
        Guid workoutExercisePlanId,
        WorkoutSetInfo workoutSetInfo,
        RestTime restTime
    )
    {
        return new WorkoutSet(userId, workoutExercisePlanId, workoutSetInfo, restTime);
    }
    #endregion

    #region Domain Methods
    public void Update(WorkoutSetInfo workoutSetInfo, RestTime restTime)
    {
        SetWorkoutSetInfo(workoutSetInfo);
        SetRestTime(restTime);
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

    private void SetWorkoutSetInfo(WorkoutSetInfo workoutSetInfo)
    {
        WorkoutSetInfo = workoutSetInfo ?? throw new DomainValidationException("WorkoutSetInfo cannot be null.");
    }

    private void SetRestTime(RestTime restTime)
    {
        RestTime = restTime ?? throw new DomainValidationException("RestTime cannot be null.");
    }
    #endregion
}
