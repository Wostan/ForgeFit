using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Domain.Events.WorkoutEvents;

public class WorkoutEntryCreatedEvent(
    Guid workoutEntryId,
    Guid workoutProgramId,
    Guid userId,
    Schedule workoutSchedule) : DomainEvent
{
    public Guid WorkoutEntryId { get; } = workoutEntryId;
    public Guid WorkoutProgramId { get; } = workoutProgramId;
    public Guid UserId { get; } = userId;
    public Schedule WorkoutSchedule { get; } = workoutSchedule;
}