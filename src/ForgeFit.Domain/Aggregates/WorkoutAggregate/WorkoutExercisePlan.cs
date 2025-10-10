using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;
using ForgeFit.Domain.ValueObjects.WorkoutValueObjects;

namespace ForgeFit.Domain.Aggregates.WorkoutAggregate;

public class WorkoutExercisePlan : EntityId, ITimeFields
{
    internal WorkoutExercisePlan(
        Guid workoutProgramId,
        WorkoutExercise workoutExercise,
        int sets,
        int reps,
        Weight weight
    )
    {
        SetWorkoutProgramId(workoutProgramId);
        SetWorkoutExercise(workoutExercise);
        SetSets(sets);
        SetReps(reps);
        SetWeight(weight);
        CreatedAt = DateTime.UtcNow;
    }

    private WorkoutExercisePlan()
    {
    }

    public Guid WorkoutProgramId { get; private set; }
    public WorkoutExercise WorkoutExercise { get; private set; }
    public int Sets { get; private set; }
    public int Reps { get; private set; }
    public Weight Weight { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public WorkoutProgram WorkoutProgram { get; private set; }

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

    private void SetSets(int sets)
    {
        if (sets < 1)
            throw new DomainValidationException("Sets must be greater than 0.");

        Sets = sets;
    }

    private void SetReps(int reps)
    {
        if (reps < 1)
            throw new DomainValidationException("Reps must be greater than 0.");

        Reps = reps;
    }

    private void SetWeight(Weight weight)
    {
        Weight = weight ?? throw new DomainValidationException("Weight cannot be null.");
    }

    public void UpdateWorkoutExercise(WorkoutExercise workoutExercise)
    {
        SetWorkoutExercise(workoutExercise);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSets(int sets)
    {
        SetSets(sets);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateReps(int reps)
    {
        SetReps(reps);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateWeight(Weight weight)
    {
        SetWeight(weight);
        UpdatedAt = DateTime.UtcNow;
    }
}