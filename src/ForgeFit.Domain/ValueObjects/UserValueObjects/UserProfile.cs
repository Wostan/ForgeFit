using ForgeFit.Domain.Enums.ProfileEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects.UserValueObjects;

public class UserProfile : ValueObject
{
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

    private UserProfile()
    {
    }

    public string Username { get; private set; }
    public Uri? AvatarUrl { get; private set; }
    public DateOfBirth DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public Weight Weight { get; private set; }
    public Height Height { get; private set; }

    private void SetUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new DomainValidationException("Username cannot be null or whitespace.");
        if (username.Length > 20)
            throw new DomainValidationException("Username must be less than 20 characters long.");

        Username = username;
    }

    private void SetAvatarUrl(Uri? avatarUrl)
    {
        if (avatarUrl is not null && !avatarUrl.IsAbsoluteUri)
            throw new DomainValidationException("AvatarUrl must be an absolute URI.");

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

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Username;
        yield return DateOfBirth;
        yield return Gender;
        yield return Weight;
        yield return Height;
    }
}
