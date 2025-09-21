using HabitsDaily.Domain.Aggregates.UserAggregate;
using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;

namespace HabitsDaily.Domain.Aggregates.PostAggregate;

public class Comment : EntityId, ITimeFields
{
    internal Comment(
        Guid postId, 
        Guid userId, 
        string textContent)
    {
        Id = Guid.NewGuid();
        SetPostId(postId);
        SetUserId(userId);
        SetTextContent(textContent);
        CreatedAt = DateTime.UtcNow;
    }
    
    private Comment() { }
    
    public Guid PostId { get; private set; }
    public Guid UserId { get; private set; }
    public string TextContent { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    
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
    
    private void SetTextContent(string textContent)
    {
        if (string.IsNullOrEmpty(textContent)) throw new DomainValidationException("TextContent cannot be null or empty.");
        if (textContent.Length > 1000) throw new DomainValidationException("TextContent cannot be longer than 1000 characters.");
        
        TextContent = textContent;
    }
    
    public void UpdateTextContent(string textContent)
    {
        SetTextContent(textContent);
        UpdatedAt = DateTime.UtcNow;
    }
}