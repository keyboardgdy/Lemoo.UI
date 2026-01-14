using Lemoo.Core.Abstractions.Domain;

namespace Lemoo.Core.Domain.Entities;

/// <summary>
/// 实体基类
/// </summary>
/// <typeparam name="TKey">主键类型</typeparam>
public abstract class EntityBase<TKey> : IEntity<TKey>
    where TKey : notnull
{
    public TKey Id { get; protected set; } = default!;

    /// <summary>
    /// Concurrency token for optimistic concurrency control
    /// </summary>
    public byte[] RowVersion { get; protected set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public string CreatedBy { get; protected set; } = string.Empty;
    public string? UpdatedBy { get; protected set; }

    public override bool Equals(object? obj)
    {
        if (obj is not EntityBase<TKey> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        // Combine type hash code with Id hash code to ensure proper equality contract
        return HashCode.Combine(GetType(), Id);
    }

    public static bool operator ==(EntityBase<TKey>? left, EntityBase<TKey>? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(EntityBase<TKey>? left, EntityBase<TKey>? right)
    {
        return !(left == right);
    }
}

/// <summary>
/// 实体基类（使用Guid作为主键）
/// </summary>
public abstract class EntityBase : EntityBase<Guid>
{
    protected EntityBase()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}

