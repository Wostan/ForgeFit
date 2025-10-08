using HabitsDaily.Domain.Aggregates.UserAggregate;
using HabitsDaily.Domain.Enums.NotificationEnums;
using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;
using HabitsDaily.Domain.ValueObjects;

namespace HabitsDaily.Domain.Aggregates.NotificationAggregate;

public class Notification : EntityId, ITimeFields
{
    internal Notification(
        Guid userId,
        NotificationType notificationType,
        string title,
        string body,
        Frequency frequency)
    {
        SetUserId(userId);
        SetNotificationType(notificationType);
        SetTitle(title);
        SetBody(body);
        SetFrequency(frequency);
        IsSent = false;
        CreatedAt = DateTime.UtcNow;
    }

    private Notification()
    {
    }

    public Guid UserId { get; private set; }
    public NotificationType NotificationType { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public Frequency Frequency { get; private set; }
    public bool IsSent { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public User User { get; private set; }

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

        if (title.Length > 20)
            throw new DomainValidationException("Title must be less than 20 characters long.");

        Title = title;
    }

    private void SetBody(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            throw new DomainValidationException("Body cannot be null or whitespace.");

        if (body.Length > 200)
            throw new DomainValidationException("Body must be less than 200 characters long.");

        Body = body;
    }
    
    private void SetFrequency(Frequency frequency)
    {
        Frequency = frequency;
    }
    
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
}