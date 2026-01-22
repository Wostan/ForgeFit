namespace ForgeFit.Domain.Aggregates;

public interface ITimeFields
{
    public DateTime CreatedAt { get; private protected init; }
    public DateTime? UpdatedAt { get; private protected set; }
}
