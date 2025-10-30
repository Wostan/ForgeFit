using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Domain.Events.UserEvents;

public class UserRegisteredEvent(Guid userId, UserProfile userProfile) : DomainEvent
{
    public Guid UserId { get; } = userId;
    public UserProfile UserProfile { get; } = userProfile;
}