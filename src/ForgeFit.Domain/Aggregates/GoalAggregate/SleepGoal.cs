using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Enums.SleepEnums;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Domain.Aggregates.GoalAggregate;

public class SleepGoal : Entity, ITimeFields
{
    internal SleepGoal(
        Guid userId,
        Schedule sleepSchedule,
        HashSet<Weekday> weekdays
    )
    {
        SetUserId(userId);
        SetWeekdays(weekdays);
        SetSleepSchedule(sleepSchedule);
        CreatedAt = DateTime.UtcNow;
    }

    private SleepGoal()
    {
    }

    public Guid UserId { get; private set; }
    public Schedule SleepSchedule { get; private set; }
    public HashSet<Weekday> Weekdays { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; private set; }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException("WorkoutProgramId cannot be empty.");

        UserId = userId;
    }

    private void SetSleepSchedule(Schedule sleepSchedule)
    {
        var duration = sleepSchedule.Duration;

        if (duration < TimeSpan.FromHours(1) || duration > TimeSpan.FromHours(14))
            throw new DomainValidationException("Sleep duration hours must be between 1 and 14 hours.");

        SleepSchedule = sleepSchedule;
    }

    private void SetWeekdays(HashSet<Weekday> weekdays)
    {
        Weekdays = weekdays;
    }

    public void UpdateGoal(
        Schedule sleepSchedule,
        HashSet<Weekday> weekdays
    )
    {
        SetSleepSchedule(sleepSchedule);
        SetWeekdays(weekdays);
        UpdatedAt = DateTime.UtcNow;
    }
}