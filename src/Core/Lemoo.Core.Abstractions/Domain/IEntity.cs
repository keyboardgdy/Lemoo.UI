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

