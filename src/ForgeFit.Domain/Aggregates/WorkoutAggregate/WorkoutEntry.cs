using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Domain.Aggregates.WorkoutAggregate;

public class WorkoutEntry : Entity
{
    internal WorkoutEntry(
        Guid workoutProgramId,
        Schedule workoutSchedule
    )
    {
        SetWorkoutProgramId(workoutProgramId);
        SetSchedule(workoutSchedule);
        CreatedAt = DateTime.UtcNow;
    }

    private WorkoutEntry()
    {
    }

    public Guid WorkoutProgramId { get; private set; }
    public Schedule WorkoutSchedule { get; private set; }
    public DateTime CreatedAt { get; init; }

    // Navigation properties
    public WorkoutProgram WorkoutProgram { get; private set; }

    private void SetWorkoutProgramId(Guid workoutProgramId)
    {
        if (workoutProgramId == Guid.Empty)
            throw new DomainValidationException("WorkoutProgramId cannot be empty.");
        WorkoutProgramId = workoutProgramId;
    }

    private void SetSchedule(Schedule workoutSchedule)
    {
        var duration = workoutSchedule.Duration;

        if (duration < TimeSpan.FromMinutes(10) || duration > TimeSpan.FromHours(5))
            throw new DomainValidationException("Workout duration hours must be between 10 minutes and 5 hours.");

        WorkoutSchedule = workoutSchedule;
    }
}