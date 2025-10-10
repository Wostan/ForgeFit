using System.Text.RegularExpressions;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.UserValueObjects;

public partial class Email : ValueObject
{
    public Email(string value)
    {
        SetEmail(value);
    }

    private Email()
    {
    }

    public string Value { get; private set; } = null!;

    private void SetEmail(string value)
    {
        if (!EmailRegex().IsMatch(value))
            throw new DomainValidationException("Invalid email format.");

        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$")]
    private static partial Regex EmailRegex();
}