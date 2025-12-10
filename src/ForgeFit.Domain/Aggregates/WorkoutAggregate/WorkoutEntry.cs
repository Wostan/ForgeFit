using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;
using ForgeFit.Domain.ValueObjects.WorkoutValueObjects;

namespace ForgeFit.Domain.Aggregates.WorkoutAggregate;

public class WorkoutEntry : Entity
{
    private readonly List<PerformedExercise> _performedExercises = [];

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

    public Guid UserId { get; private set; }
    public Guid WorkoutProgramId { get; private set; }
    public Schedule WorkoutSchedule { get; private set; }
    public DateTime CreatedAt { get; init; }

    // Navigation properties
    public User User { get; private set; }
    public WorkoutProgram WorkoutProgram { get; private set; }
    public ICollection<PerformedExercise> PerformedExercises => _performedExercises.AsReadOnly();

    public static WorkoutEntry Create(
        Guid userId,
        Guid workoutProgramId,
        Schedule workoutSchedule,
        ICollection<PerformedExercise> performedExercises
    )
    {
        return new WorkoutEntry(userId, workoutProgramId, workoutSchedule, performedExercises);
    }

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

        if (duration < TimeSpan.FromMinutes(10) || duration > TimeSpan.FromHours(5))
            throw new DomainValidationException("Workout duration hours must be between 10 minutes and 5 hours.");
        WorkoutSchedule = workoutSchedule;
    }

    private void SetPerformedExercises(ICollection<PerformedExercise> performedExercises)
    {
        if (performedExercises == null || performedExercises.Count == 0)
            throw new DomainValidationException("PerformedExercises cannot be empty.");
        _performedExercises.AddRange(performedExercises);
    }
    
    public void UpdatePerformedExercises(ICollection<PerformedExercise> performedExercises)
    {
        if (performedExercises is null || performedExercises.Count == 0)
            throw new DomainValidationException("PerformedExercises cannot be empty.");
        _performedExercises.Clear();
        _performedExercises.AddRange(performedExercises);
    }

    public void Update(Schedule schedule, ICollection<PerformedExercise> performedExercises)
    {
        SetSchedule(schedule);
        UpdatePerformedExercises(performedExercises);
    }
}
