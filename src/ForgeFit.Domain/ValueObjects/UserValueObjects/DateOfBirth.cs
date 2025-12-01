using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.UserValueObjects;

public class DateOfBirth : ValueObject
{
    public static readonly DateTime EarliestBirthDate = DateTime.UtcNow.AddYears(-100);
    public static readonly DateTime LatestBirthDate = DateTime.UtcNow.AddYears(-13);

    public DateOfBirth(DateTime value)
    {
        SetDate(value);
    }

    public DateTime Value { get; private set; }

    private void SetDate(DateTime value)
    {
        if (value > LatestBirthDate || value < EarliestBirthDate)
            throw new DomainValidationException("The minimum user age is 13 years.");

        Value = value;
    }

    public int GetAge()
    {
        var today = DateTime.UtcNow;
        var age = today.Year - Value.Year;
        if (Value.Date > today.AddYears(-age)) age--;
        return age;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}