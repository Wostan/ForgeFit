using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects.UserValueObjects;
using Email = ForgeFit.Domain.ValueObjects.UserValueObjects.Email;

namespace ForgeFit.Domain.Aggregates.UserAggregate;

public class User : Entity, ITimeFields
{
    #region Private Fields
    #endregion

    #region Constructors
    internal User(UserProfile userProfile, Email email, string passwordHash)
    {
        SetUserProfile(userProfile);
        SetEmail(email);
        SetPasswordHash(passwordHash);
        CreatedAt = DateTime.UtcNow;
    }

    private User()
    {
    }
    #endregion

    #region Public Properties
    public UserProfile UserProfile { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    #endregion

    #region Navigation Properties
    // Navigation properties are handled by EF Core through collections in other entities
    #endregion

    #region Factory Methods
    public static User Create(UserProfile userProfile, Email email, string passwordHash)
    {
        return new User(userProfile, email, passwordHash);
    }
    #endregion

    #region Domain Methods
    public void UpdateUserProfile(UserProfile userProfile)
    {
        SetUserProfile(userProfile);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(Email email)
    {
        SetEmail(email);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        SetPasswordHash(passwordHash);
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion

    #region Private Setters
    private void SetUserProfile(UserProfile userProfile)
    {
        UserProfile = userProfile ?? throw new DomainValidationException("UserProfile cannot be null.");
    }

    private void SetEmail(Email email)
    {
        Email = email ?? throw new DomainValidationException("Email cannot be null.");
    }

    private void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash ?? throw new DomainValidationException("PasswordHash cannot be null.");
        
        if (passwordHash.Length > DomainConstants.ValidationLimits.MaxPasswordHashLength)
            throw new DomainValidationException($"Password hash cannot exceed {DomainConstants.ValidationLimits.MaxPasswordHashLength} characters.");
    }
    #endregion
}
