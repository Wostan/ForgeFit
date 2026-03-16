using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Constants;
using ForgeFit.Domain.Enums.NotificationEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Domain.Aggregates.NotificationAggregate;

public class Notification : Entity, ITimeFields
{
    #region Private Fields
    #endregion

    #region Constructors
    internal Notification(
        Guid userId,
        NotificationType notificationType,
        string title,
        string body,
        Frequency frequency,
        TimeOnly scheduledAt)
    {
        SetUserId(userId);
        SetNotificationType(notificationType);
        SetTitle(title);
        SetBody(body);
        SetFrequency(frequency);
        SetScheduledAt(scheduledAt);
        IsSent = false;
        CreatedAt = DateTime.UtcNow;
    }

    private Notification()
    {
    }
    #endregion

    #region Public Properties
    public Guid UserId { get; private set; }
    public NotificationType NotificationType { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public Frequency Frequency { get; private set; }
    public TimeOnly ScheduledAt { get; private set; }
    public bool IsSent { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    #endregion

    #region Navigation Properties
    public User User { get; private set; }
    #endregion

    #region Factory Methods
    public static Notification Create(
        Guid userId,
        NotificationType notificationType,
        string title,
        string body,
        Frequency frequency,
        TimeOnly scheduledAt)
    {
        return new Notification(userId, notificationType, title, body, frequency, scheduledAt);
    }
    #endregion

    #region Domain Methods
    public void MarkAsSent()
    {
        if (IsSent)
            throw new DomainValidationException("Notification is already marked as sent.");
        IsSent = true;
    }

    public void UpdateInfo(
        NotificationType notificationType,
        string title,
        string body,
        Frequency frequency
    )
    {
        SetNotificationType(notificationType);
        SetTitle(title);
        SetBody(body);
        SetFrequency(frequency);
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

    private void SetNotificationType(NotificationType notificationType)
    {
        if (!Enum.IsDefined(notificationType))
            throw new DomainValidationException("NotificationType is not defined.");

        NotificationType = notificationType;
    }

    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainValidationException("Title cannot be null or whitespace.");

        if (title.Length > DomainConstants.ValidationLimits.MaxTitleLength)
            throw new DomainValidationException($"Title must be less than {DomainConstants.ValidationLimits.MaxTitleLength} characters long.");

        Title = title;
    }

    private void SetBody(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            throw new DomainValidationException("Body cannot be null or whitespace.");

        if (body.Length > DomainConstants.ValidationLimits.MaxDescriptionLength)
            throw new DomainValidationException($"Body must be less than {DomainConstants.ValidationLimits.MaxDescriptionLength} characters long.");

        Body = body;
    }

    private void SetFrequency(Frequency frequency)
    {
        Frequency = frequency;
    }

    private void SetScheduledAt(TimeOnly scheduledAt)
    {
        ScheduledAt = scheduledAt;
    }
    #endregion
}
