namespace HabitsDaily.Domain.Primitives;

public abstract class EntityId : IEquatable<EntityId>
{
    public Guid Id { get; protected init; }

    public bool Equals(EntityId? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((EntityId)obj);
    }
    
    public static bool operator ==(EntityId? left, EntityId? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;

        return left.Equals(right);
    }

    public static bool operator !=(EntityId? left, EntityId? right)
        => !(left == right);


    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}