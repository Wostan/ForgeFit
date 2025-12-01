using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects.WorkoutValueObjects;

namespace ForgeFit.Domain.Aggregates.WorkoutAggregate;

public class WorkoutExercisePlan : Entity, ITimeFields
{
    internal WorkoutExercisePlan(
        Guid workoutProgramId,
        WorkoutExercise workoutExercise,
        ICollection<WorkoutSet> workoutSets)
    {
        SetWorkoutProgramId(workoutProgramId);
        SetWorkoutExercise(workoutExercise);
        SetWorkoutSets(workoutSets);
        CreatedAt = DateTime.UtcNow;
    }

    private WorkoutExercisePlan()
    {
    }

    public Guid WorkoutProgramId { get; private set; }
    public WorkoutExercise WorkoutExercise { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public WorkoutProgram WorkoutProgram { get; private set; }
    public ICollection<WorkoutSet> WorkoutSets { get; private set; }

    public static WorkoutExercisePlan Create(
        Guid workoutProgramId,
        WorkoutExercise workoutExercise,
        ICollection<WorkoutSet> workoutSets)
    {
        return new WorkoutExercisePlan(workoutProgramId, workoutExercise, workoutSets);
    }

    private void SetWorkoutProgramId(Guid workoutProgramId)
    {
        if (workoutProgramId == Guid.Empty)
            throw new DomainValidationException("Workout program ID cannot be empty.");

        WorkoutProgramId = workoutProgramId;
    }

    private void SetWorkoutExercise(WorkoutExercise workoutExercise)
    {
        WorkoutExercise = workoutExercise ?? throw new DomainValidationException("Workout exercise cannot be null.");
    }

    private void SetWorkoutSets(ICollection<WorkoutSet> workoutSets)
    {
        WorkoutSets = workoutSets ?? throw new DomainValidationException("Workout sets cannot be null.");
    }

    public void UpdateWorkoutExercise(WorkoutExercise workoutExercise)
    {
        SetWorkoutExercise(workoutExercise);
        UpdatedAt = DateTime.UtcNow;
    }
}
