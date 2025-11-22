using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Aggregates.UserAggregate;

public class RefreshToken : Entity, ITimeFields
{
    internal RefreshToken(
        Guid userId, 
        string token, 
        DateTime expiryDate)
    {
        UserId = userId;
        Token = token;
        ExpiryDate = expiryDate;
        CreatedAt = DateTime.UtcNow;
    }

    private RefreshToken()
    {
    }

    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public bool IsRevoked { get; private set; } = false;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public User User { get; private set; }
    
    public static RefreshToken Create(
        Guid userId, 
        string token, 
        DateTime expiryDate)
    {
        return new RefreshToken(userId, token, expiryDate);
    }

    public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiryDate;
    public void Revoke()
    {
        IsRevoked = true;
        UpdatedAt = DateTime.UtcNow;
    }
}