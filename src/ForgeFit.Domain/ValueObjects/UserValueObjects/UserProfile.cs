using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Enums.ProfileEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.UserValueObjects;

public class UserProfile : ValueObject
{
    #region Constructors
    public UserProfile(
        string username,
        Uri? avatarUrl,
        DateOfBirth dateOfBirth,
        Gender gender,
        Weight weight,
        Height height
    )
    {
        SetUsername(username);
        SetAvatarUrl(avatarUrl);
        SetDateOfBirth(dateOfBirth);
        SetGender(gender);
        SetWeight(weight);
        SetHeight(height);
    }
    
    private UserProfile() { }
    #endregion

    #region Public Properties
    public string Username { get; private set; }
    public Uri? AvatarUrl { get; private set; }
    public DateOfBirth DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public Weight Weight { get; private set; }
    public Height Height { get; private set; }
    #endregion

    #region Private Methods
    private void SetUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new DomainValidationException("Username cannot be null or whitespace.");
        if (username.Length > DomainConstants.ValidationLimits.MaxUsernameLength)
            throw new DomainValidationException($"Username must be less than {DomainConstants.ValidationLimits.MaxUsernameLength} characters long.");

        Username = username;
    }

    private void SetAvatarUrl(Uri? avatarUrl)
    {
        if (avatarUrl != null)
        {
            if (!avatarUrl.IsAbsoluteUri)
                throw new DomainValidationException("AvatarUrl must be an absolute URI.");
            
            if (avatarUrl.ToString().Length > DomainConstants.ValidationLimits.MaxAvatarUrlLength)
                throw new DomainValidationException($"AvatarUrl cannot exceed {DomainConstants.ValidationLimits.MaxAvatarUrlLength} characters.");
        }

        AvatarUrl = avatarUrl;
    }

    private void SetDateOfBirth(DateOfBirth dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
    }

    private void SetGender(Gender gender)
    {
        if (!Enum.IsDefined(gender))
            throw new DomainValidationException("Gender is not defined.");

        Gender = gender;
    }

    private void SetWeight(Weight weight)
    {
        Weight = weight;
    }

    private void SetHeight(Height height)
    {
        Height = height;
    }
    #endregion

    #region ValueObject Implementation
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Username;
        yield return DateOfBirth;
        yield return Gender;
        yield return Weight;
        yield return Height;
    }
    #endregion
}
