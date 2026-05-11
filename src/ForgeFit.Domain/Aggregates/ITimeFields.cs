namespace ForgeFit.Domain.Aggregates;

public interface ITimeFields
{
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }
}
