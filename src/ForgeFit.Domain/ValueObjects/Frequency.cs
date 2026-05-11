using ForgeFit.Domain.Enums.HabitEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects;

public class Frequency : ValueObject
{
    #region Constructors
    public Frequency(int interval, FrequencyUnit frequencyUnit)
    {
        SetInterval(interval);
        SetFrequencyUnit(frequencyUnit);
    }
    #endregion

    #region Public Properties
    public int Interval { get; private set; }
    public FrequencyUnit FrequencyUnit { get; private set; }
    #endregion

    #region Private Methods
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
    #endregion

    #region Public Methods
    public static Frequency Create(int interval, FrequencyUnit frequencyUnit) => new(interval, frequencyUnit);
    #endregion

    #region ValueObject Implementation
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Interval;
        yield return FrequencyUnit;
    }
    #endregion
}
