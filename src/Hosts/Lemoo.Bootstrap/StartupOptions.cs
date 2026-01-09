namespace Lemoo.Bootstrap;

/// <summary>
/// 启动配置选项
/// </summary>
public class StartupOptions
{
    /// <summary>
    /// 是否启用自动数据库迁移
    /// </summary>
    public bool AutoMigrate { get; set; } = false;
    
    /// <summary>
    /// 是否启用数据种子
    /// </summary>
    public bool SeedData { get; set; } = false;
    
    /// <summary>
    /// 是否在启动失败时抛出异常
    /// </summary>
    public bool ThrowOnBootstrapFailure { get; set; } = true;
    
    /// <summary>
    /// 是否启用模块生命周期日志
    /// </summary>
    public bool EnableModuleLifecycleLogging { get; set; } = true;
}

