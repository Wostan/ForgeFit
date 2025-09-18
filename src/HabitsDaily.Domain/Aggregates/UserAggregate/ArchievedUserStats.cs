using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;

namespace HabitsDaily.Domain.Aggregates.UserAggregate;

public class ArchievedUserStats : Entity, ITimeFields
{
    internal ArchievedUserStats(
        Guid userId,
        Guid habitId,
        int totalPoints)
    {
        Id = Guid.NewGuid();
        SetUserId(userId);
        SetHabitId(habitId);
        SetTotalPoints(totalPoints);
        CreatedAt = DateTime.UtcNow;
    }
    
    private ArchievedUserStats() { }
    
    public Guid UserId { get; private set; }
    public Guid HabitId { get; private set; }
    public int TotalPoints { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty) throw new DomainValidationException("UserId cannot be empty.");
        
        UserId = userId;
    }

    private void SetHabitId(Guid habitId)
    {
        if (habitId == Guid.Empty) throw new DomainValidationException("HabitId cannot be empty.");
        
        HabitId = habitId;
    }
    
    private void SetTotalPoints(int totalPoints)
    {
        if (totalPoints < 0) throw new DomainValidationException("TotalPoints cannot be negative.");
        
        TotalPoints = totalPoints;
    }
    
    public void AddPoints(int points)
    {
        if (points < 0) throw new DomainValidationException("Points cannot be negative.");
        
        TotalPoints += points;
        UpdatedAt = DateTime.UtcNow;
    }
}