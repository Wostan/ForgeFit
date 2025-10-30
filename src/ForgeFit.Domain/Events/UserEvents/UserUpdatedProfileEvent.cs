using ForgeFit.Domain.Primitives.Interfaces;
using ForgeFit.Domain.ValueObjects.UserValueObjects;

namespace ForgeFit.Domain.Events.UserEvents;

public class UserUpdatedProfileEvent(Guid userId, UserProfile userProfile) : DomainEvent
{
    public Guid UserId { get; } = userId;
    public UserProfile UserProfile { get; } = userProfile;
}