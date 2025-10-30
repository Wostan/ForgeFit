using ForgeFit.Domain.Primitives.Interfaces;
using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Domain.Events.WorkoutEvents;

public class WorkoutEntryUpdatedEvent(
    Guid workoutEntryId,
    Schedule workoutSchedule) : DomainEvent
{
    public Guid WorkoutEntryId { get; } = workoutEntryId;
    public Schedule WorkoutSchedule { get; } = workoutSchedule;
}