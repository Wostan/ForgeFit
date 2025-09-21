using HabitsDaily.Domain.Aggregates.UserAggregate;
using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;

namespace HabitsDaily.Domain.Aggregates.PostAggregate;

public class Like : EntityId
{
    internal Like(Guid postId, Guid userId)
    {
        Id = Guid.NewGuid();
        SetPostId(postId);
        SetUserId(userId);
        CreatedAt = DateTime.UtcNow;
    }
    
    private Like() { }
    
    public Guid PostId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; init; }
    
    // Navigation properties
    public Post Post { get; private set; }
    public User User { get; private set; }

    private void SetPostId(Guid postId)
    {
        if (postId == Guid.Empty) throw new DomainValidationException("PostId cannot be empty.");
        
        PostId = postId;
    }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty) throw new DomainValidationException("UserId cannot be empty.");
        
        UserId = userId;
    }
}