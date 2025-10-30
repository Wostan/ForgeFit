using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Events.WorkoutEvents;

public class WorkoutProgramCreatedEvent(
    Guid workoutProgramId,
    Guid userId) : DomainEvent
{
    public Guid WorkoutProgramId { get; } = workoutProgramId;
    public Guid UserId { get; } = userId;
}