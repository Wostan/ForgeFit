using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.WorkoutValueObjects;

public class PerformedExercise : ValueObject
{
    #region Constructors
    public PerformedExercise(WorkoutExercise snapshot, ICollection<PerformedSet> sets)
    {
        SetSnapshot(snapshot);
        SetSets(sets);
    }

    private PerformedExercise()
    {
    }
    #endregion

    #region Public Properties
    public WorkoutExercise Snapshot { get; private set; }
    #endregion

    #region Navigation Properties
    public ICollection<PerformedSet> Sets { get; private set; }
    #endregion

    #region Private Methods
    private void SetSnapshot(WorkoutExercise snapshot)
    {
        Snapshot = snapshot ?? throw new DomainValidationException("Exercise snapshot cannot be null");
    }

    private void SetSets(ICollection<PerformedSet> sets)
    {
        if (sets == null || sets.Count == 0) throw new DomainValidationException("Sets cannot be null");
        Sets = sets;
    }
    #endregion

    #region ValueObject Implementation
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Snapshot;
        foreach (var set in Sets) yield return set;
    }
    #endregion
}
