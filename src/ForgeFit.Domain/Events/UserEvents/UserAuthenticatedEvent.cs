using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Events.UserEvents;

public class UserAuthenticatedEvent(Guid userId) : DomainEvent
{
    public Guid UserId { get; } = userId;
}
