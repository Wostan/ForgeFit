using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;

namespace ForgeFit.Domain.Aggregates.HabitAggregate;

public class HabitRecord : EntityId
{
    internal HabitRecord(
        Guid habitId,
        DateTime dueDate)
    {
        SetHabitId(habitId);
        SetDueDate(dueDate);
        Completed = false;
    }

    private HabitRecord()
    {
    }

    public Guid HabitId { get; private set; }
    public DateTime DueDate { get; private set; }
    public bool Completed { get; private set; }

    // Navigation properties
    public Habit Habit { get; private set; }

    private void SetHabitId(Guid habitId)
    {
        if (habitId == Guid.Empty)
            throw new DomainValidationException("HabitId cannot be empty.");

        HabitId = habitId;
    }

    public void SetDueDate(DateTime dueDate)
    {
        if (dueDate.Date < DateTime.UtcNow.Date)
            throw new DomainValidationException("Date cannot be in the past.");

        DueDate = dueDate.Date;
    }

    public void MarkAsDone()
    {
        if (Completed)
            throw new DomainValidationException("HabitRecord already completed.");
        Completed = true;
    }

    public void MarkAsUndone()
    {
        if (!Completed)
            throw new DomainValidationException("HabitRecord is not completed.");

        Completed = false;
    }
}