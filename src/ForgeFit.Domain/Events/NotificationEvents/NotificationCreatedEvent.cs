using ForgeFit.Domain.Enums.NotificationEnums;
using ForgeFit.Domain.Primitives.Interfaces;
using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Domain.Events.NotificationEvents;

public class NotificationCreatedEvent(
    Guid notificationId,
    Guid userId,
    Frequency frequency,
    NotificationType notificationType,
    TimeOnly scheduledAt) : DomainEvent
{
    public Guid NotificationId { get; } = notificationId;
    public Guid UserId { get; } = userId;
    public Frequency Frequency { get; } = frequency;
    public NotificationType NotificationType { get; } = notificationType;
    public TimeOnly ScheduledAt { get; } = scheduledAt;
}