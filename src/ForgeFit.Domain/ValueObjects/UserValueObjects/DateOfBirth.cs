using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.UserValueObjects;

public class DateOfBirth : ValueObject
{
    #region Constructors
    public DateOfBirth(DateTime value)
    {
        SetDate(value);
    }
    #endregion

    #region Public Properties
    public DateTime Value { get; private set; }
    #endregion

    #region Private Methods
    private void SetDate(DateTime value)
    {
        var today = DateTime.UtcNow;
        var earliestBirthDate = today.AddYears(-DomainConstants.ValidationLimits.MaxAgeYears);
        var latestBirthDate = today.AddYears(-DomainConstants.ValidationLimits.MinAgeYears);
        
        if (value > latestBirthDate || value < earliestBirthDate)
            throw new DomainValidationException($"The minimum user age is {DomainConstants.ValidationLimits.MinAgeYears} years.");

        Value = value;
    }
    #endregion

    #region Public Methods
    public static DateOfBirth Create(DateTime value) => new(value);
    
    public int GetAge()
    {
        var today = DateTime.UtcNow;
        var age = today.Year - Value.Year;
        if (Value.Date > today.AddYears(-age)) age--;
        return age;
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd");
    #endregion

    #region ValueObject Implementation
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
    #endregion
}
