using HabitsDaily.Domain.Aggregates.UserAggregate;
using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;

namespace HabitsDaily.Domain.Aggregates.StreakAggregate;

public class Streak : EntityId, ITimeFields
{
    internal Streak(Guid userId)
    {
        Id = Guid.NewGuid();
        SetUserId(userId);
        Level = StreakLevel.Bronze;
        CurrentStreakDays = 1;
        CreatedAt = DateTime.UtcNow;
    }
    
    private Streak() { }
    
    public Guid UserId { get; private set; }
    public StreakLevel Level { get; private set; }  
    public int CurrentStreakDays { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public User User { get; private set; }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty) throw new DomainValidationException("UserId cannot be empty.");
        
        UserId = userId;
    }

    public void ChangeLevel(StreakLevel level)
    {
        if (!Enum.IsDefined(level)) throw new DomainValidationException("Invalid 'StreakLevel' value.");
        
        Level = level;
    }
    
    public void IncrementStreakDays(int days = 1)
    {
        if (days <= 0) throw new DomainValidationException("Days must be positive.");
        CurrentStreakDays += days;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum StreakLevel
{
    Bronze = 1,
    Silver,
    Gold,
    Platinum
}