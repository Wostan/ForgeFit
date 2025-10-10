namespace ForgeFit.Domain.Primitives;

public abstract class EntityId : IEquatable<EntityId>
{
    public Guid Id { get; } = Guid.NewGuid();

    public bool Equals(EntityId? other)
    {
        if (other is null) return false;
        return ReferenceEquals(this, other) || Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((EntityId)obj);
    }

    public static bool operator ==(EntityId? left, EntityId? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;

        return left.Equals(right);
    }

    public static bool operator !=(EntityId? left, EntityId? right)
    {
        return !(left == right);
    }


    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}