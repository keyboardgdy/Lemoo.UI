using Microsoft.EntityFrameworkCore;

namespace Lemoo.Core.Abstractions.Persistence;

/// <summary>
/// 模块数据库上下文工厂接口
/// </summary>
public interface IModuleDbContextFactory
{
    /// <summary>
    /// 创建数据库上下文
    /// </summary>
    TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext;
    
    /// <summary>
    /// 根据模块名称创建数据库上下文
    /// </summary>
    DbContext CreateDbContext(string moduleName);
    
    /// <summary>
    /// 注册数据库上下文
    /// </summary>
    void RegisterDbContext<TDbContext>(string connectionString, DatabaseProvider provider)
        where TDbContext : DbContext;
}

/// <summary>
/// 数据库提供程序枚举
/// </summary>
public enum DatabaseProvider
{
    SqlServer,
    PostgreSQL,
    SQLite,
    InMemory
}

