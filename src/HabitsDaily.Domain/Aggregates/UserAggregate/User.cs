using System.Text.Json.Serialization;
using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;
using HabitsDaily.Domain.ValueObjects;

namespace HabitsDaily.Domain.Aggregates.UserAggregate;

public class User : Entity, ITimeFields
{
    internal User(
        string username,
        Email email,
        DateOfBirth dateOfBirth,
        string passwordHash,
        Uri? avatarUrl)
    {
        Id = Guid.NewGuid();
        SetUsername(username);
        SetEmail(email);
        SetDateOfBirth(dateOfBirth);
        SetPasswordHash(passwordHash);
        SetAvatarUrl(avatarUrl);
        CreatedAt = DateTime.UtcNow;
    }

    [JsonConstructor]
    private User() { }
    
    public string Username { get; private set; }
    public Email Email { get; private set; }
    public DateOfBirth DateOfBirth { get; private set; }
    public string PasswordHash { get; private set; }
    public Uri? AvatarUrl { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    public void SetUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new DomainValidationException("Username cannot be null or whitespace.");
        }

        if (username.Length is < 3 or > 20)
        {
            throw new DomainValidationException("Username must be between 3 and 20 characters long.");
        }
        
        Username = username;
    }
    
    public void UpdateUsername(string username)
    {
        SetUsername(username);
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetEmail(Email email)
    {
        Email = email ?? throw new DomainValidationException("Email cannot be null.");
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDateOfBirth(DateOfBirth dateOfBirth)
    {
        DateOfBirth = dateOfBirth ?? throw new DomainValidationException("DateOfBirth cannot be null.");
        UpdatedAt = DateTime.UtcNow;
    }    
    
    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash ?? throw new DomainValidationException("PasswordHash cannot be null.");
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAvatarUrl(Uri? avatarUrl)
    {
        AvatarUrl = avatarUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}