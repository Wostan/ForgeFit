using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;
using ForgeFit.Domain.ValueObjects.WorkoutValueObjects;

namespace ForgeFit.Domain.Aggregates.WorkoutAggregate;

public class WorkoutEntry : Entity, ITimeFields
{
    #region Private Fields
    private readonly List<PerformedExercise> _performedExercises = [];
    #endregion

    #region Constructors
    internal WorkoutEntry(
        Guid userId,
        Guid workoutProgramId,
        Schedule workoutSchedule,
        ICollection<PerformedExercise> performedExercises
    )
    {
        SetUserId(userId);
        SetWorkoutProgramId(workoutProgramId);
        SetSchedule(workoutSchedule);
        SetPerformedExercises(performedExercises);
        CreatedAt = DateTime.UtcNow;
    }

    private WorkoutEntry()
    {
    }
    #endregion

    #region Public Properties
    public Guid UserId { get; private set; }
    public Guid WorkoutProgramId { get; private set; }
    public Schedule WorkoutSchedule { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    #endregion

    #region Navigation Properties
    public User User { get; private set; }
    public WorkoutProgram WorkoutProgram { get; private set; }
    public IReadOnlyCollection<PerformedExercise> PerformedExercises => _performedExercises.AsReadOnly();
    #endregion

    #region Factory Methods
    public static WorkoutEntry Create(
        Guid userId,
        Guid workoutProgramId,
        Schedule workoutSchedule,
        ICollection<PerformedExercise> performedExercises
    )
    {
        return new WorkoutEntry(userId, workoutProgramId, workoutSchedule, performedExercises);
    }
    #endregion

    #region Domain Methods
    public void Update(Schedule schedule)
    {
        SetSchedule(schedule);
        UpdatedAt = DateTime.UtcNow;
    }


    public void Update(Schedule schedule, ICollection<PerformedExercise> performedExercises)
    {
        SetSchedule(schedule);
        SetPerformedExercises(performedExercises);
        UpdatedAt = DateTime.UtcNow;
    }

    #endregion

    #region Private Setters
    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty) throw new DomainValidationException("UserId cannot be empty.");
        UserId = userId;
    }

    private void SetWorkoutProgramId(Guid workoutProgramId)
    {
        if (workoutProgramId == Guid.Empty) throw new DomainValidationException("WorkoutProgramId cannot be empty.");
        WorkoutProgramId = workoutProgramId;
    }

    private void SetSchedule(Schedule workoutSchedule)
    {
        var duration = workoutSchedule.Duration;

        if (duration < TimeSpan.FromMinutes(DomainConstants.ValidationLimits.MinWorkoutDurationMinutes) || 
            duration > TimeSpan.FromHours(DomainConstants.ValidationLimits.MaxWorkoutDurationHours))
            throw new DomainValidationException($"Workout duration must be between {DomainConstants.ValidationLimits.MinWorkoutDurationMinutes} minutes and {DomainConstants.ValidationLimits.MaxWorkoutDurationHours} hours.");
        WorkoutSchedule = workoutSchedule;
    }

    private void SetPerformedExercises(ICollection<PerformedExercise> performedExercises)
    {
        if (performedExercises == null || performedExercises.Count == 0)
            throw new DomainValidationException("PerformedExercises cannot be empty.");
            
        _performedExercises.Clear();
        foreach (var exercise in performedExercises)
        {
            if (_performedExercises.Contains(exercise))
                throw new DomainValidationException("Performed exercise already exists.");
            _performedExercises.Add(exercise);
        }
    }
    #endregion
}
