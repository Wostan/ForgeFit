using System.Text.Json.Serialization;
using HabitsDaily.Domain.Aggregates.UserAggregate;
using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;
using HabitsDaily.Domain.ValueObjects;

namespace HabitsDaily.Domain.Aggregates.HabitAggregate;

public class Habit : EntityId, ITimeFields
{
    internal Habit(
        Guid userId,
        string name,
        string? description,
        Frequency frequency)
    {
        Id = Guid.NewGuid();
        SetUserId(userId);
        SetName(name);
        SetDescription(description);
        SetFrequency(frequency);
        CreatedAt = DateTime.UtcNow;
    }
    
    private Habit() { }
    
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Frequency Frequency { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public User User { get; private set; }
    
    public List<HabitRecord> HabitRecords { get; private set; } = [];
    public List<ArchivedUserStats> ArchivedUserStats { get; private set; } = [];
    
    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty) throw new DomainValidationException("UserId cannot be empty.");
        
        UserId = userId;
    }
    
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainValidationException("Name cannot be null or whitespace.");
        }
        
        if (name.Length > 50)
        {
            throw new DomainValidationException("Name must be less than 50 characters long.");
        }
        
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetDescription(string? description)
    {
        if (description?.Length > 300)
        {
            throw new DomainValidationException("Description must be less than 300 characters long.");
        }
        
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFrequency(Frequency frequency)
    {
        Frequency = frequency ?? throw new DomainValidationException("Frequency cannot be null.");
        UpdatedAt = DateTime.UtcNow;
    }
}