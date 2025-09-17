namespace HabitsDaily.Domain.Aggregates;

public interface ITimeFields
{
    public DateTime CreatedAt { get; protected init; }
    public DateTime? UpdatedAt { get; protected set; }
}