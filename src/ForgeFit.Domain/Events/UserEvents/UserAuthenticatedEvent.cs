using ForgeFit.Domain.Primitives.Interfaces;

namespace ForgeFit.Domain.Events.UserEvents;

public class UserAuthenticatedEvent(Guid userId) : DomainEvent
{
    public Guid UserId { get; } = userId;
}