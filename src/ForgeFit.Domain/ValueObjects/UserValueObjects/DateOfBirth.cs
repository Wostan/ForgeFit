using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.UserValueObjects;

public class DateOfBirth : ValueObject
{
    private static readonly DateTime MinDate = new(1900, 1, 1);
    private static readonly DateTime MaxDate = DateTime.UtcNow.AddYears(-6);

    public DateOfBirth(DateTime value)
    {
        SetDate(value);
    }

    public DateTime Value { get; private set; }

    private void SetDate(DateTime value)
    {
        if (value < MinDate || value > MaxDate)
            throw new DomainValidationException("The minimum user age is 6 years.");

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