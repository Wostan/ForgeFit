using System.Text.Json.Serialization;
using HabitsDaily.Domain.Aggregates.HabitAggregate;
using HabitsDaily.Domain.Aggregates.PostAggregate;
using HabitsDaily.Domain.Aggregates.ShopAggregate;
using HabitsDaily.Domain.Aggregates.StreakAggregate;
using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;
using HabitsDaily.Domain.ValueObjects;
using Email = HabitsDaily.Domain.ValueObjects.Email;

namespace HabitsDaily.Domain.Aggregates.UserAggregate;

public class User : EntityId, ITimeFields
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

    private User() { }
    
    public string Username { get; private set; }
    public Email Email { get; private set; }
    public DateOfBirth DateOfBirth { get; private set; }
    public string PasswordHash { get; private set; }
    public Uri? AvatarUrl { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public List<Habit> Habits { get; private set; } = [];
    public List<ArchivedUserStats> ArchivedUserStats { get; private set; } = [];
    public Streak Streak { get; private set; }
    
    public List<Post> Posts { get; private set; } = [];
    public List<Like> Likes { get; private set; } = [];
    public List<Comment> Comments { get; private set; } = [];
    
    public List<Purchase> Purchases { get; private set; } = [];
    
    public List<Friend> Friends { get; private set; } = [];

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
        if (avatarUrl is not null && !avatarUrl.IsAbsoluteUri) throw new DomainValidationException("AvatarUrl must be an absolute URI.");
        
        AvatarUrl = avatarUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}