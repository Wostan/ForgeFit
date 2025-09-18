using HabitsDaily.Domain.Exceptions;
using HabitsDaily.Domain.Primitives;

namespace HabitsDaily.Domain.Aggregates.PostAggregate;

public class Post : Entity, ITimeFields
{
    internal Post(
        Guid userId,
        string textContent,
        Uri? mediaUrl
    )
    {
        Id = Guid.NewGuid();
        SetUserId(userId);
        SetTextContent(textContent);
        SetMediaUrl(mediaUrl);
        ViewsCount = 0;
        CreatedAt = DateTime.UtcNow;
    }
    
    private Post() { }
    
    public Guid UserId { get; private set; }
    public string TextContent { get; private set; }
    public Uri? MediaUrl { get; private set; }
    public int ViewsCount { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    
    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty) throw new DomainValidationException("UserId cannot be empty.");
        
        UserId = userId;
    }
    
    private void SetTextContent(string textContent)
    {
        if (string.IsNullOrEmpty(textContent)) throw new DomainValidationException("TextContent cannot be null or empty.");
        if (textContent.Length > 2000) throw new DomainValidationException("TextContent cannot be longer than 2000 characters.");
        
        TextContent = textContent;
    }
    
    private void SetMediaUrl(Uri? mediaUrl)
    {
        if (mediaUrl is not null && !mediaUrl.IsAbsoluteUri) throw new DomainValidationException("MediaUrl must be an absolute URI.");
        
        MediaUrl = mediaUrl;
    }
    
    public void IncrementViews()
    {
        ViewsCount++;
        UpdatedAt = DateTime.UtcNow;
    }
}