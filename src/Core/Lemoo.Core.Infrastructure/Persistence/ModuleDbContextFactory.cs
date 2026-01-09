using Lemoo.Core.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Infrastructure.Persistence;

/// <summary>
/// 模块数据库上下文工厂实现
/// </summary>
public class ModuleDbContextFactory : IModuleDbContextFactory
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ModuleDbContextFactory> _logger;
    private readonly Dictionary<Type, DbContextOptions> _optionsCache = new();
    private readonly Dictionary<string, Type> _moduleDbContextMap = new();
    
    public ModuleDbContextFactory(IConfiguration configuration, ILogger<ModuleDbContextFactory> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    
    public TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext
    {
        var dbContextType = typeof(TDbContext);
        
        if (!_optionsCache.TryGetValue(dbContextType, out var options))
        {
            options = CreateOptions<TDbContext>();
            _optionsCache[dbContextType] = options;
        }
        
        return (TDbContext)Activator.CreateInstance(dbContextType, options)!;
    }
    
    private DbContextOptions CreateOptions<TDbContext>() where TDbContext : DbContext
    {
        var moduleName = ExtractModuleName<TDbContext>();
        var connectionString = _configuration.GetConnectionString(moduleName)
            ?? throw new InvalidOperationException($"未找到模块 '{moduleName}' 的连接字符串");
            
        var provider = _configuration.GetValue<DatabaseProvider>(
            "Lemoo:Database:Provider",
            DatabaseProvider.SqlServer);
        
        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
        
        switch (provider)
        {
            case DatabaseProvider.SqlServer:
                optionsBuilder.UseSqlServer(connectionString, options =>
                {
                    options.MigrationsAssembly($"{typeof(TDbContext).Namespace}");
                    options.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
                break;
            case DatabaseProvider.PostgreSQL:
                // 需要添加 Npgsql.EntityFrameworkCore.PostgreSQL 包
                // optionsBuilder.UseNpgsql(connectionString);
                throw new NotSupportedException("PostgreSQL 提供程序需要额外的 NuGet 包");
            case DatabaseProvider.SQLite:
                // 需要添加 Microsoft.EntityFrameworkCore.Sqlite 包
                // optionsBuilder.UseSqlite(connectionString);
                throw new NotSupportedException("SQLite 提供程序需要额外的 NuGet 包");
            case DatabaseProvider.InMemory:
                // 需要添加 Microsoft.EntityFrameworkCore.InMemory 包
                // optionsBuilder.UseInMemoryDatabase(connectionString);
                throw new NotSupportedException("InMemory 提供程序需要额外的 NuGet 包");
            default:
                throw new NotSupportedException($"不支持的数据库提供程序: {provider}");
        }
        
        return optionsBuilder.Options;
    }
    
    private string ExtractModuleName<TDbContext>() where TDbContext : DbContext
    {
        var typeName = typeof(TDbContext).Name;
        return typeName.Replace("DbContext", "");
    }
    
    public DbContext CreateDbContext(string moduleName)
    {
        if (!_moduleDbContextMap.TryGetValue(moduleName, out var dbContextType))
            throw new InvalidOperationException($"未找到模块 '{moduleName}' 的DbContext类型");
            
        if (!_optionsCache.TryGetValue(dbContextType, out var options))
        {
            // 需要根据模块名称创建选项
            throw new InvalidOperationException($"模块 '{moduleName}' 的DbContext选项未配置");
        }
        
        return (DbContext)Activator.CreateInstance(dbContextType, options)!;
    }
    
    public void RegisterDbContext<TDbContext>(string connectionString, DatabaseProvider provider)
        where TDbContext : DbContext
    {
        var moduleName = ExtractModuleName<TDbContext>();
        _moduleDbContextMap[moduleName] = typeof(TDbContext);
        
        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
        
        switch (provider)
        {
            case DatabaseProvider.SqlServer:
                optionsBuilder.UseSqlServer(connectionString);
                break;
            default:
                throw new NotSupportedException($"不支持的数据库提供程序: {provider}");
        }
        
        _optionsCache[typeof(TDbContext)] = optionsBuilder.Options;
        
        _logger.LogInformation("注册数据库上下文: {ModuleName}", moduleName);
    }
}

