using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Events.HabitEvents;

public class HabitCreatedEvent(Guid habitId, Guid userId) : DomainEvent
{
    public Guid HabitId { get; } = habitId;
    public Guid UserId { get; } = userId;
}
