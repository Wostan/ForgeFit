using ForgeFit.Domain.Primitives.Interfaces;

namespace ForgeFit.Domain.Events.NotificationEvents;

public class NotificationSentEvent(Guid notificationId, Guid userId) : DomainEvent
{
    public Guid NotificationId { get; } = notificationId;
    public Guid UserId { get; } = userId;
}