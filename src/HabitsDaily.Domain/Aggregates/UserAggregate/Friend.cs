using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;

namespace HabitsDaily.Domain.Aggregates.UserAggregate;

public class Friend : ITimeFields
{
    internal Friend(
        Guid userId,
        Guid friendId,
        FriendStatus status)
    {
        SetUserId(userId);
        SetFriendId(friendId);
        SetStatus(status);
        CreatedAt = DateTime.UtcNow;
    }
    
    private Friend() { }
    
    public Guid UserId { get; private set; }
    public Guid FriendId { get; private set; }
    public FriendStatus Status { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    
    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty) throw new DomainValidationException("UserId cannot be empty.");
        
        UserId = userId;
    }

    private void SetFriendId(Guid friendId)
    {
        if (friendId == Guid.Empty) throw new DomainValidationException("FriendId cannot be empty.");
        if (friendId == UserId) throw new DomainValidationException("FriendId cannot be the same as UserId.");
        
        FriendId = friendId;
    }

    public void SetStatus(FriendStatus status)
    {
        if (!Enum.IsDefined(status)) throw new DomainValidationException("Invalid 'FriendStatus' value.");
        
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum FriendStatus
{
    Pending = 1,
    Accepted,
    Declined,
    Blocked
}