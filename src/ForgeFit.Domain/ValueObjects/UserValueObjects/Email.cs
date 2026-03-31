using System.Text.RegularExpressions;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.UserValueObjects;

public partial class Email : ValueObject
{
    #region Constructors
    public Email(string value)
    {
        SetEmail(value);
    }
    
    private Email() { }
    #endregion

    #region Public Properties
    public string Value { get; private set; } = null!;
    #endregion

    #region Private Methods
    private void SetEmail(string value)
    {
        if (!EmailRegex().IsMatch(value))
            throw new DomainValidationException("Invalid email format.");
        
        if (value.Length > DomainConstants.ValidationLimits.MaxEmailLength)
            throw new DomainValidationException($"Email cannot exceed {DomainConstants.ValidationLimits.MaxEmailLength} characters.");

        Value = value;
    }
    #endregion

    #region Public Methods
    public static Email Create(string value) => new(value);
    
    public override string ToString() => Value;
    #endregion

    #region ValueObject Implementation
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
    #endregion

    #region Private Members
    [GeneratedRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$")]
    private static partial Regex EmailRegex();
    #endregion
}
