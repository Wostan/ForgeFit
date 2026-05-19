using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.ValueObjects;

public class Schedule : ValueObject
{
    #region Constructors
    public Schedule(TimeOnly start, TimeOnly end)
    {
        Start = start;
        End = end;
        SetDurationHours();
    }
    #endregion

    #region Public Properties
    public TimeOnly Start { get; }
    public TimeOnly End { get; }
    public TimeSpan Duration { get; private set; }
    #endregion

    #region Private Methods
    private void SetDurationHours()
    {
        TimeSpan duration;

        if (End < Start)
            duration = End.AddHours(24) - Start;
        else
            duration = End - Start;

        Duration = duration;
    }
    #endregion

    #region Public Methods
    public static Schedule Create(TimeOnly start, TimeOnly end) => new(start, end);
    
    public override string ToString() => $"{Start:HH:mm} - {End:HH:mm} ({Duration:hh\\:mm})";
    #endregion

    #region ValueObject Implementation
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
    #endregion
}
