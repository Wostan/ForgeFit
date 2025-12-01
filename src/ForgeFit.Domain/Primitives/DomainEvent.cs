using MediatR;

namespace ForgeFit.Domain.Primitives;

public abstract class DomainEvent : INotification
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
