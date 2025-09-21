using System.Text.RegularExpressions;
using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;

namespace HabitsDaily.Domain.ValueObjects;

public partial class Email : ValueObject
{
    public Email(string value)
    {
        SetEmail(value);
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