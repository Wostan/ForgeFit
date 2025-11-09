using ForgeFit.Domain.Aggregates.GoalAggregate;
using ForgeFit.Domain.Enums.GoalEnums;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Domain.Events.GoalEvents;

public class BodyGoalUpdatedEvent(
    Guid goalId, 
    Weight weightGoal, 
    DateTime? dueDate, 
    GoalType goalType, 
    GoalStatus goalStatus) : DomainEvent
{
    public Guid GoalId { get; } = goalId;
    public Weight WeightGoal { get; } = weightGoal;
    public DateTime? DueDate { get; } = dueDate;
    public GoalType GoalType { get; } = goalType;
    public GoalStatus GoalStatus { get; } = goalStatus;
}