using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.WorkoutValueObjects;

public class PerformedExercise : ValueObject
{
    public PerformedExercise(WorkoutExercise snapshot, ICollection<PerformedSet> sets)
    {
        SetSnapshot(snapshot);
        SetSets(sets);
    }

    public WorkoutExercise Snapshot { get; private set; }
    
    // Navigation properties
    public ICollection<PerformedSet> Sets { get; private set; }
    
    private void SetSnapshot(WorkoutExercise snapshot)
    {
        Snapshot = snapshot ?? throw new DomainValidationException("Exercise snapshot cannot be null");
    }
    
    private void SetSets(ICollection<PerformedSet> sets)
    {
        if (sets == null || sets.Count == 0)
        {
            throw new DomainValidationException("Sets cannot be null");
        }
        Sets = sets;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Snapshot;
        foreach (var set in Sets) yield return set;
    }
}