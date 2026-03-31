using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.WorkoutValueObjects;

public class PerformedSet : ValueObject
{
    #region Constructors
    public PerformedSet(int order, int reps, Weight weight, bool isCompleted)
    {
        SetOrder(order);
        SetReps(reps);
        SetWeight(weight);
        IsCompleted = isCompleted;
    }

    private PerformedSet() { }
    #endregion

    #region Public Properties
    public int Order { get; private set; }
    public int Reps { get; private set; }
    public Weight Weight { get; private set; }
    public bool IsCompleted { get; private set; }
    #endregion

    #region Private Methods
    private void SetOrder(int order)
    {
        if (order < 0) throw new DomainValidationException("Order cannot be negative.");
        Order = order;
    }

    private void SetReps(int reps)
    {
        if (reps < 0) throw new DomainValidationException("Reps cannot be negative.");
        Reps = reps;
    }

    private void SetWeight(Weight weight)
    {
        Weight = weight ?? throw new DomainValidationException("Weight cannot be null.");
    }
    #endregion

    #region ValueObject Implementation
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Order;
        yield return Reps;
        yield return Weight;
        yield return IsCompleted;
    }
    #endregion
}
