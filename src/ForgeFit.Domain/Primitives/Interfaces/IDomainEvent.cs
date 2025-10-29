using MediatR;

namespace ForgeFit.Domain.Primitives.Interfaces;

public abstract class DomainEvent : INotification
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}