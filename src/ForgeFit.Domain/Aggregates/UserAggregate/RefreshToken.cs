using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Aggregates.UserAggregate;

public class RefreshToken : Entity, ITimeFields
{
    #region Private Fields
    #endregion

    #region Constructors
    internal RefreshToken(Guid userId, string token, DateTime expiryDate)
    {
        SetUserId(userId);
        SetToken(token);
        SetExpiryDate(expiryDate);
        CreatedAt = DateTime.UtcNow;
    }

    private RefreshToken()
    {
    }
    #endregion

    #region Public Properties
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsActive => DateTime.UtcNow < ExpiryDate;
    #endregion

    #region Navigation Properties
    public User User { get; private set; }
    #endregion

    #region Factory Methods
    public static RefreshToken Create(Guid userId, string token, DateTime expiryDate)
    {
        return new RefreshToken(userId, token, expiryDate);
    }
    #endregion

    #region Domain Methods
    public void Update(RefreshToken refreshToken)
    {
        SetToken(refreshToken.Token);
        SetExpiryDate(refreshToken.ExpiryDate);
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion

    #region Private Setters
    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException("UserId cannot be empty.");

        UserId = userId;
    }

    private void SetToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new DomainValidationException("Token cannot be null or whitespace.");
        
        if (token.Length > DomainConstants.ValidationLimits.MaxRefreshTokenLength)
            throw new DomainValidationException($"Token cannot exceed {DomainConstants.ValidationLimits.MaxRefreshTokenLength} characters.");

        Token = token;
    }

    private void SetExpiryDate(DateTime expiryDate)
    {
        if (expiryDate <= DateTime.UtcNow)
            throw new DomainValidationException("ExpiryDate must be in the future.");

        ExpiryDate = expiryDate;
    }
    #endregion
}
