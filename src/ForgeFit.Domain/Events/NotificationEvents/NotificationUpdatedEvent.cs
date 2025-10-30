using ForgeFit.Domain.Enums.NotificationEnums;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Domain.Events.NotificationEvents;

public class NotificationUpdatedEvent(
    Guid notificationId,
    Frequency frequency,
    NotificationType notificationType,
    TimeOnly scheduledAt) : DomainEvent
{
    public Guid NotificationId { get; } = notificationId;
    public Frequency Frequency { get; } = frequency;
    public NotificationType NotificationType { get; } = notificationType;
    public TimeOnly ScheduledAt { get; } = scheduledAt;
}