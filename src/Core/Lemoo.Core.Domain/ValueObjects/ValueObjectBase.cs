namespace Lemoo.Core.Domain.ValueObjects;

/// <summary>
/// 值对象基类
/// </summary>
public abstract class ValueObjectBase : IEquatable<ValueObjectBase>
{
    /// <summary>
    /// 获取用于相等性比较的组件
    /// </summary>
    protected abstract IEnumerable<object> GetEqualityComponents();
    
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;
            
        var other = (ValueObjectBase)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }
    
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }
    
    public bool Equals(ValueObjectBase? other)
    {
        return Equals((object?)other);
    }
    
    public static bool operator ==(ValueObjectBase? left, ValueObjectBase? right)
    {
        if (left is null && right is null)
            return true;
            
        if (left is null || right is null)
            return false;
            
        return left.Equals(right);
    }
    
    public static bool operator !=(ValueObjectBase? left, ValueObjectBase? right)
    {
        return !(left == right);
    }
}

