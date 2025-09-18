using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;

namespace HabitsDaily.Domain.ValueObjects;

public class Frequency : ValueObject
{
    public int Interval { get; }
    public Unit Unit { get; }
    
    private Frequency() { }
    
    public Frequency(int interval, Unit unit)
    {
        if (interval <= 0) throw new DomainValidationException("Interval must be positive.");
        if (!Enum.IsDefined(unit)) throw new DomainValidationException("Invalid unit value.");

        Interval = interval;
        Unit = unit;
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Interval;
        yield return Unit;
    }
}

public enum Unit
{
    Daily = 1, 
    Weekly,
    Monthly
}
