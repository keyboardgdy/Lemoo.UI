namespace Lemoo.Core.Domain.ValueObjects;

/// <summary>
/// 值对象基类 - DDD 核心概念之一
/// 值对象通过其属性值来标识，而不是通过 ID
/// 值对象是不可变的
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// 获取用于相等性比较的原子值
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    /// 值对象相等性比较 - 基于组件值而非引用
    /// </summary>
    public bool Equals(ValueObject? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ValueObject);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}

/// <summary>
/// 泛型值对象基类
/// </summary>
/// <typeparam name="TValue">值对象的基础值类型</typeparam>
public abstract class ValueObject<TValue> : ValueObject
    where TValue : notnull
{
    /// <summary>
    /// 值对象的基础值
    /// </summary>
    public TValue Value { get; }

    protected ValueObject(TValue value)
    {
        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
