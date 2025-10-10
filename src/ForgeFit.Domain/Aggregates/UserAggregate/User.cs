using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects.UserValueObjects;
using Email = ForgeFit.Domain.ValueObjects.UserValueObjects.Email;

namespace ForgeFit.Domain.Aggregates.UserAggregate;

public class User : EntityId, ITimeFields
{
    internal User(
        UserProfile userProfile,
        Email email,
        string passwordHash)
    {
        SetUserProfile(userProfile);
        SetEmail(email);
        SetPasswordHash(passwordHash);
        CreatedAt = DateTime.UtcNow;
    }

    private User()
    {
    }

    public UserProfile UserProfile { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

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
    }

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
}