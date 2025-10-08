using HabitsDaily.Domain.Aggregates.UserAggregate;
using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;
using HabitsDaily.Domain.ValueObjects;

namespace HabitsDaily.Domain.Aggregates.SleepAggregate;

public class SleepEntry : EntityId
{
    internal SleepEntry(
        Guid userId,
        Schedule sleepSchedule
    )
    {
        SetUserId(userId);
        SetSleepSchedule(sleepSchedule);
        SetSleepDate();
        CreatedAt = DateTime.UtcNow;
    }

    private SleepEntry()
    {
    }

    public Guid UserId { get; private set; }
    public Schedule SleepSchedule { get; private set; }
    public DateOnly SleepDate { get; private set; }
    public DateTime CreatedAt { get; init; }

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

        SleepSchedule = sleepSchedule ?? throw new DomainValidationException("Schedule cannot be null.");
    }

    private void SetSleepDate()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var bedtime = SleepSchedule.Start;
        var wakeTime = SleepSchedule.End;

        SleepDate = wakeTime < bedtime ? today.AddDays(-1) : today;
    }
}