using ForgeFit.Domain.Primitives.Interfaces;

namespace ForgeFit.Domain.Events.HabitEvents;

public class HabitCompletedEvent(Guid habitId, Guid userId) : DomainEvent
{
    public Guid HabitId { get; } = habitId;
    public Guid UserId { get; } = userId;
}