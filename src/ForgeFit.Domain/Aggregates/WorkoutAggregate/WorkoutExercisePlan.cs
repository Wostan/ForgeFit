using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects.WorkoutValueObjects;

namespace ForgeFit.Domain.Aggregates.WorkoutAggregate;

public class WorkoutExercisePlan : Entity, ITimeFields
{
    #region Private Fields
    private readonly List<WorkoutSet> _workoutSets = [];
    #endregion

    #region Constructors
    internal WorkoutExercisePlan(
        Guid workoutProgramId,
        WorkoutExercise workoutExercise,
        ICollection<WorkoutSet> workoutSets
    )
    {
        SetWorkoutProgramId(workoutProgramId);
        SetWorkoutExercise(workoutExercise);
        SetWorkoutSets(workoutSets);
        CreatedAt = DateTime.UtcNow;
    }

    private WorkoutExercisePlan()
    {
    }
    #endregion

    #region Public Properties
    public Guid WorkoutProgramId { get; private set; }
    public WorkoutExercise WorkoutExercise { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    #endregion

    #region Navigation Properties
    public WorkoutProgram WorkoutProgram { get; private set; }
    public IReadOnlyCollection<WorkoutSet> WorkoutSets => _workoutSets.AsReadOnly();
    #endregion

    #region Factory Methods
    public static WorkoutExercisePlan Create(
        Guid workoutProgramId,
        WorkoutExercise workoutExercise,
        ICollection<WorkoutSet> workoutSets
    )
    {
        return new WorkoutExercisePlan(workoutProgramId, workoutExercise, workoutSets);
    }
    #endregion

    #region Domain Methods
    public void UpdateWorkoutExercise(WorkoutExercise workoutExercise)
    {
        SetWorkoutExercise(workoutExercise);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddSet(WorkoutSet set)
    {
        if (set is null) throw new DomainValidationException("Set cannot be null.");
        _workoutSets.Add(set);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveSet(WorkoutSet set)
    {
        _workoutSets.Remove(set);
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion

    #region Private Setters
    private void SetWorkoutProgramId(Guid workoutProgramId)
    {
        if (workoutProgramId == Guid.Empty) throw new DomainValidationException("WorkoutProgramId required");
        WorkoutProgramId = workoutProgramId;
    }

    private void SetWorkoutExercise(WorkoutExercise workoutExercise)
    {
        WorkoutExercise = workoutExercise ?? throw new DomainValidationException("WorkoutExercise cannot be null");
    }

    private void SetWorkoutSets(ICollection<WorkoutSet> workoutSets)
    {
        if (workoutSets is null) throw new DomainValidationException("Workout sets cannot be null.");
        
        if (workoutSets.Count > DomainConstants.ValidationLimits.MaxSetsPerExercise)
            throw new DomainValidationException($"Cannot exceed {DomainConstants.ValidationLimits.MaxSetsPerExercise} sets per exercise.");

        _workoutSets.Clear();
        _workoutSets.AddRange(workoutSets);
    }
    #endregion
}
