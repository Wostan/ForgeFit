using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects;

public class Schedule : ValueObject
{
    public Schedule(TimeOnly start, TimeOnly end)
    {
        Start = start;
        End = end;
        SetDurationHours();
    }

    private Schedule()
    {
    }

    public TimeOnly Start { get; }
    public TimeOnly End { get; }
    public TimeSpan Duration { get; private set; }

    private void SetDurationHours()
    {
        TimeSpan duration;

        if (End < Start)
            duration = End.AddHours(24) - Start;
        else
            duration = End - Start;

        Duration = duration;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
}