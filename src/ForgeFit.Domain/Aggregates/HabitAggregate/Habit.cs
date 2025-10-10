using ForgeFit.Domain.Aggregates.UserAggregate;
using ForgeFit.Domain.Exceptions;
using ForgeFit.Domain.Primitives;
using ForgeFit.Domain.ValueObjects;

namespace ForgeFit.Domain.Aggregates.HabitAggregate;

public class Habit : EntityId, ITimeFields
{
    internal Habit(
        Guid userId,
        string name,
        string? description,
        Frequency frequency)
    {
        SetUserId(userId);
        SetName(name);
        SetDescription(description);
        SetFrequency(frequency);
        IsActive = true;
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
    }

    private Habit()
    {
    }

    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Frequency Frequency { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; private set; }
    public ICollection<HabitRecord> HabitRecords { get; }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty) throw new DomainValidationException("WorkoutProgramId cannot be empty.");

        UserId = userId;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainValidationException("Name cannot be null or whitespace.");

        if (name.Length > 50) throw new DomainValidationException("Name must be less than 50 characters long.");

        Name = name;
    }

    private void SetDescription(string? description)
    {
        if (description?.Length > 300)
            throw new DomainValidationException("Description must be less than 300 characters long.");

        Description = description;
    }

    private void SetFrequency(Frequency frequency)
    {
        Frequency = frequency ?? throw new DomainValidationException("Frequency cannot be null.");
    }

    public void UpdateInfo(string name, string? description, Frequency frequency)
    {
        SetName(name);
        SetDescription(description);
        SetFrequency(frequency);
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDone(Guid habitRecordId)
    {
        var habitRecord = HabitRecords.FirstOrDefault(hr => hr.Id == habitRecordId);
        if (habitRecord is null)
            throw new DomainValidationException("HabitRecord not found.");

        habitRecord.MarkAsDone();
    }

    public void MarkAsUndone(Guid habitRecordId)
    {
        var habitRecord = HabitRecords.FirstOrDefault(hr => hr.Id == habitRecordId);
        if (habitRecord is null)
            throw new DomainValidationException("HabitRecord not found.");

        habitRecord.MarkAsUndone();
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainValidationException("Habit is already deactivated.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (IsDeleted)
            throw new DomainValidationException("Habit is already deleted.");

        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}