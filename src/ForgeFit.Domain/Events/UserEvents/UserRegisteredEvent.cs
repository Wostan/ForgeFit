using ForgeFit.Domain.Primitives.Interfaces;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Domain.Events.UserEvents;

public class UserRegisteredEvent(Guid userId, UserProfile userProfile) : IDomainEvent
{
    public Guid UserId { get; } = userId;
    public UserProfile UserProfile { get; } = userProfile;
}