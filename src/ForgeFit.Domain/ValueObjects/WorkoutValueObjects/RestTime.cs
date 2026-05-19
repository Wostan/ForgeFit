using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.WorkoutValueObjects;

public class RestTime : ValueObject
{
    #region Constructors
    public RestTime(TimeSpan value)
    {
        SetValue(value);
    }
    
    private RestTime() { }
    #endregion

    #region Public Properties
    public TimeSpan Value { get; private set; }
    #endregion

    #region Private Methods
    private void SetValue(TimeSpan value)
    {
        if (value < TimeSpan.Zero || value > TimeSpan.FromMinutes(DomainConstants.ValidationLimits.MaxRestTimeMinutes))
            throw new DomainValidationException($"Rest time must be between 0 and {DomainConstants.ValidationLimits.MaxRestTimeMinutes} minutes.");

        Value = value;
    }
    #endregion

    #region Public Methods
    public static RestTime FromSeconds(int seconds) => new(TimeSpan.FromSeconds(seconds));
    public static RestTime FromMinutes(int minutes) => new(TimeSpan.FromMinutes(minutes));
    public static RestTime Zero => new(TimeSpan.Zero);

    public override string ToString() => Value.TotalSeconds switch
    {
        < DomainConstants.Time.SecondsPerMinute => $"{Value.TotalSeconds:F0}s",
        < DomainConstants.Time.SecondsPerHour => $"{Value.TotalMinutes:F1}m",
        _ => $"{Value.TotalHours:F1}h"
    };
    #endregion

    #region ValueObject Implementation
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
    #endregion
}
