namespace Lemoo.Core.Abstractions.Domain;

/// <summary>
/// 实体接口
/// </summary>
/// <typeparam name="TKey">主键类型</typeparam>
public interface IEntity<out TKey>
{
    TKey Id { get; }
}

/// <summary>
/// 实体接口（默认使用Guid作为主键）
/// </summary>
public interface IEntity : IEntity<Guid>
{
}

/// <summary>
/// 聚合根接口 - 标记一个实体为聚合根
/// 聚合根是领域模型中的一致性边界
/// </summary>
/// <typeparam name="TKey">主键类型</typeparam>
public interface IAggregateRoot<out TKey> : IEntity<TKey>, IDomainEventContainer
{
}

/// <summary>
/// 聚合根接口（使用 Guid 作为主键）
/// </summary>
public interface IAggregateRoot : IAggregateRoot<Guid>
{
}

