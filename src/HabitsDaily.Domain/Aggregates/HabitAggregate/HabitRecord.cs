using System.Text.Json.Serialization;
using HabitsDaily.Domain.Aggregates.UserAggregate;
using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;

namespace HabitsDaily.Domain.Aggregates.HabitAggregate;

public class HabitRecord : EntityId
{
    internal HabitRecord(
        Guid habitId,
        DateTime date,
        bool completed,
        int pointsEarned)
    {
        Id = Guid.NewGuid();
        SetHabitId(habitId);
        SetDate(date);
        SetCompleted(completed);
        SetPointsEarned(pointsEarned);
    }
    
    private HabitRecord() { }
    
    public Guid HabitId { get; private set; }
    public DateTime Date { get; private set; }
    public bool Completed { get; private set; }
    public int PointsEarned { get; private set; }
    
    // Navigation properties
    public Habit Habit { get; private set; }
    public User User { get; private set; }

    public void SetHabitId(Guid habitId)
    {
        if (habitId == Guid.Empty) 
            throw new DomainValidationException("HabitId cannot be empty.");

        HabitId = habitId;
    }

    public void SetDate(DateTime date)
    {
        if (date.Date > DateTime.UtcNow.Date)
            throw new DomainValidationException("Date cannot be in the future.");

        Date = date.Date;
    }

    public void SetCompleted(bool completed)
    {
        Completed = completed;
    }

    public void SetPointsEarned(int points)
    {
        if (points < 0) 
            throw new DomainValidationException("Points cannot be negative.");

        PointsEarned = points;
    }
}