using ForgeFit.Domain.Enums.HabitEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects;

public class Frequency : ValueObject
{
    public Frequency(int interval, FrequencyUnit frequencyUnit)
    {
        SetInterval(interval);
        SetFrequencyUnit(frequencyUnit);
    }

    public int Interval { get; private set; }
    public FrequencyUnit FrequencyUnit { get; private set; }

    private void SetInterval(int interval)
    {
        if (interval <= 0)
            throw new DomainValidationException("Interval must be positive.");

        Interval = interval;
    }

    private void SetFrequencyUnit(FrequencyUnit frequencyUnit)
    {
        if (!Enum.IsDefined(FrequencyUnit))
            throw new DomainValidationException("FrequencyUnit is not defined.");

        FrequencyUnit = frequencyUnit;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Interval;
        yield return FrequencyUnit;
    }
}