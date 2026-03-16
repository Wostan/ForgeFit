using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.WorkoutValueObjects;

public class WorkoutSetInfo : ValueObject
{
    #region Constructors
    public WorkoutSetInfo(int order, int reps, Weight weight)
    {
        SetOrder(order);
        SetReps(reps);
        SetWeight(weight);
    }
    #endregion

    #region Public Properties
    public int Order { get; private set; }
    public int Reps { get; private set; }
    public Weight Weight { get; private set; }
    #endregion

    #region Private Methods
    private void SetOrder(int order)
    {
        if (order < 1)
            throw new DomainValidationException("Order must be greater than 0.");

        Order = order;
    }

    private void SetReps(int reps)
    {
        if (reps is < 1 or > DomainConstants.ValidationLimits.MaxRepsPerSet)
            throw new DomainValidationException($"Reps must be between 1 and {DomainConstants.ValidationLimits.MaxRepsPerSet}.");

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
    }
    #endregion
}
