using ForgeFit.Domain.Enums.WorkoutEnums;
using ForgeFit.Domain.Primitives.Interfaces;
using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Domain.Events.GoalEvents;

public class WorkoutGoalUpdated(
    Guid workoutGoalId,
    int workoutsPerWeek, 
    Schedule schedule,
    WorkoutType workoutType) : DomainEvent
{
    public Guid WorkoutGoalId { get; } = workoutGoalId;
    public int WorkoutsPerWeek { get; } = workoutsPerWeek;
    public Schedule Schedule { get; } = schedule;
    public WorkoutType WorkoutType { get; } = workoutType;
}