namespace Lemoo.Core.Abstractions.Domain;

/// <summary>
/// 领域服务标记接口
///
/// 领域服务用于处理：
/// - 涉及多个聚合根的操作
/// - 不自然的领域逻辑（无法自然放在某个聚合或值对象中）
/// - 需要访问基础设施的领域逻辑
/// - 复杂的业务规则计算
///
/// 与应用服务的区别：
/// - 领域服务包含纯领域逻辑，是无状态的
/// - 应用服务协调用例，处理事务边界
/// </summary>
public interface IDomainService
{
}

/// <summary>
/// 领域服务基类 - 提供通用的依赖和方法
/// </summary>
public abstract class DomainService : IDomainService
{
    // 可以在这里添加通用的依赖或方法
    // 例如：ILogger, IDomainEventDispatcher 等
}

/// <summary>
/// 泛型领域服务基类
/// </summary>
/// <typeparam name="T">相关的实体类型</typeparam>
public abstract class DomainService<T> : DomainService
    where T : class
{
    // 可以添加针对特定实体类型的通用方法
}
