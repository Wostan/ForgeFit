using System.Text.Json.Serialization;

namespace HabitsDaily.Domain.Aggregates.UserAggregate;

public class User
{
    [JsonConstructor]
    private User() { }
    
    public int Id { get; set; }
    public string Username { get; set; }
}