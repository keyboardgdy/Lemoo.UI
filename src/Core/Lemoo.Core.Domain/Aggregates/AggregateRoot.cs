using Lemoo.Core.Abstractions.Domain;
using Lemoo.Core.Domain.Entities;

namespace Lemoo.Core.Domain.Aggregates;

/// <summary>
/// 聚合根基类
/// </summary>
/// <typeparam name="TKey">主键类型</typeparam>
public abstract class AggregateRoot<TKey> : EntityBase<TKey>
    where TKey : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();
    
    /// <summary>
    /// 领域事件集合
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    /// <summary>
    /// 添加领域事件
    /// </summary>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    /// <summary>
    /// 移除领域事件
    /// </summary>
    protected void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }
    
    /// <summary>
    /// 清空领域事件
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

/// <summary>
/// 聚合根基类（使用Guid作为主键）
/// </summary>
public abstract class AggregateRoot : AggregateRoot<Guid>
{
    /// <summary>
    /// 默认构造函数，初始化聚合根的 Id 和创建时间
    /// </summary>
    protected AggregateRoot()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}

