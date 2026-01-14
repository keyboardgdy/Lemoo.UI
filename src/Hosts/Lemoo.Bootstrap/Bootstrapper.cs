using System.Diagnostics;
using Lemoo.Core.Abstractions.Caching;
using Lemoo.Core.Abstractions.Configuration;
using Lemoo.Core.Abstractions.Deployment;
using Lemoo.Core.Abstractions.Module;
using Lemoo.Core.Abstractions.Persistence;
using Lemoo.Core.Infrastructure.Caching;
using Lemoo.Core.Infrastructure.Configuration;
using Lemoo.Core.Infrastructure.Deployment;
using Lemoo.Core.Infrastructure.Logging;
using Lemoo.Core.Infrastructure.ModuleLoader;
using Lemoo.Core.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Lemoo.Bootstrap;

/// <summary>
/// 启动引导器实现
/// </summary>
public class Bootstrapper : IBootstrapper
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Bootstrapper> _logger;
    private readonly StartupOptions _options;
    private IReadOnlyList<IModule>? _loadedModules;

    public Bootstrapper(IConfiguration configuration, ILogger<Bootstrapper> logger, StartupOptions? options = null)
    {
        _configuration = configuration;
        _logger = logger;
        _options = options ?? new StartupOptions();
    }

    /// <summary>
    /// 获取启动选项
    /// </summary>
    public StartupOptions Options => _options;
    
    public async Task<BootstrapResult> BootstrapAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var errors = new List<BootstrapError>();
        
        try
        {
            _logger.LogInformation("开始应用程序引导...");
            
            // 1. 验证配置
            var configErrors = ValidateConfiguration();
            errors.AddRange(configErrors);
            
            if (errors.Any())
            {
                stopwatch.Stop();
                return new BootstrapResult
                {
                    IsSuccess = false,
                    Errors = errors,
                    ElapsedTime = stopwatch.Elapsed,
                    Message = "配置验证失败"
                };
            }
            
            // 2. 检测环境
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
                             ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") 
                             ?? "Production";
            _logger.LogInformation("当前环境: {Environment}", environment);
            
            // 3. 初始化日志（已在ConfigureHost中配置）
            
            _logger.LogInformation("应用程序引导完成，耗时: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
            
            stopwatch.Stop();
            return new BootstrapResult
            {
                IsSuccess = true,
                Errors = Array.Empty<BootstrapError>(),
                ElapsedTime = stopwatch.Elapsed,
                Message = "应用程序引导成功"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "应用程序引导失败");
            errors.Add(new BootstrapError
            {
                Code = "BOOTSTRAP_ERROR",
                Message = ex.Message,
                Details = ex.ToString()
            });
            
            stopwatch.Stop();
            return new BootstrapResult
            {
                IsSuccess = false,
                Errors = errors,
                ElapsedTime = stopwatch.Elapsed,
                Message = "应用程序引导失败"
            };
        }
    }
    
    /// <summary>
    /// 启动模块生命周期
    /// </summary>
    public async Task StartModulesAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (_loadedModules == null)
        {
            _logger.LogWarning("模块尚未加载，无法启动模块生命周期");
            return;
        }

        var moduleLoader = serviceProvider.GetRequiredService<IModuleLoader>();
        var modules = moduleLoader.GetLoadedModules();

        // 启动前
        foreach (var module in modules)
        {
            try
            {
                if (_options.EnableModuleLifecycleLogging)
                {
                    _logger.LogDebug("模块 {ModuleName} 启动前处理", module.Name);
                }
                await module.OnApplicationStartingAsync(serviceProvider, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "模块 {ModuleName} 启动前处理失败", module.Name);
                throw;
            }
        }

        // 启动后
        foreach (var module in modules)
        {
            try
            {
                if (_options.EnableModuleLifecycleLogging)
                {
                    _logger.LogDebug("模块 {ModuleName} 启动后处理", module.Name);
                }
                await module.OnApplicationStartedAsync(serviceProvider, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "模块 {ModuleName} 启动后处理失败", module.Name);
                throw;
            }
        }

        _logger.LogInformation("所有模块生命周期启动完成");
    }
    
    /// <summary>
    /// 停止模块生命周期
    /// </summary>
    public async Task StopModulesAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (_loadedModules == null)
        {
            return;
        }

        var moduleLoader = serviceProvider.GetRequiredService<IModuleLoader>();
        var modules = moduleLoader.GetLoadedModules().Reverse();

        // 停止前
        foreach (var module in modules)
        {
            try
            {
                if (_options.EnableModuleLifecycleLogging)
                {
                    _logger.LogDebug("模块 {ModuleName} 停止前处理", module.Name);
                }
                await module.OnApplicationStoppingAsync(serviceProvider, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "模块 {ModuleName} 停止前处理失败", module.Name);
            }
        }

        // 停止后
        foreach (var module in modules)
        {
            try
            {
                if (_options.EnableModuleLifecycleLogging)
                {
                    _logger.LogDebug("模块 {ModuleName} 停止后处理", module.Name);
                }
                await module.OnApplicationStoppedAsync(serviceProvider, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "模块 {ModuleName} 停止后处理失败", module.Name);
            }
        }

        _logger.LogInformation("所有模块生命周期停止完成");
    }
    
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        _logger.LogInformation("注册核心服务（同步模式）...");

        // 注册基础设施服务
        services.AddSingleton<IModuleLoader, ModuleLoader>();
        services.AddSingleton<IModuleContractRegistry, ModuleContractRegistry>();
        services.AddMemoryCache();
        services.AddLogging(builder =>
        {
            builder.AddSerilog();
        });

        // 加载并注册模块
        // 注意：这里需要先注册ILogger，所以使用临时服务提供者
        var tempServices = new ServiceCollection();
        tempServices.AddSingleton<IConfiguration>(configuration);
        tempServices.AddLogging();

        // 使用 using 确保临时 ServiceProvider 被正确释放
        using var tempProvider = tempServices.BuildServiceProvider();

        var moduleLoader = new ModuleLoader(
            tempProvider.GetRequiredService<ILogger<ModuleLoader>>(),
            configuration);

        // 同步加载模块（在服务注册阶段）
        // 注意：由于 RegisterServices 不是异步方法，必须使用 GetAwaiter().GetResult()
        // 使用 Task.Run 确保异步操作在线程池线程上执行，避免死锁
        try
        {
            _loadedModules = Task.Run(() => moduleLoader.LoadModulesAsync()).GetAwaiter().GetResult();
        }
        catch (AggregateException ex)
        {
            // 展开聚合异常以获取实际异常
            if (ex.InnerExceptions.Count == 1)
            {
                throw ex.InnerExceptions[0];
            }
            throw;
        }

        // 获取模块契约注册表
        var contractRegistry = new ModuleContractRegistry();

        // 按顺序配置模块服务（使用同步方法）
        foreach (var module in _loadedModules)
        {
            _logger.LogInformation("配置模块服务: {ModuleName}", module.Name);
            module.PreConfigureServices(services, configuration);
            module.ConfigureServices(services, configuration);
        }

        // 后配置（按依赖逆序）
        foreach (var module in _loadedModules.Reverse())
        {
            module.PostConfigureServices(services, configuration);
        }

        // 注册契约注册表
        services.AddSingleton(contractRegistry);

        _logger.LogInformation("已注册 {Count} 个模块", _loadedModules.Count);
    }

    /// <summary>
    /// 异步注册服务（推荐使用）
    /// </summary>
    public async Task RegisterServicesAsync(IServiceCollection services, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("注册核心服务（异步模式）...");

        // 注册基础设施服务
        services.AddSingleton<IModuleLoader, ModuleLoader>();
        services.AddSingleton<IModuleContractRegistry, ModuleContractRegistry>();
        services.AddMemoryCache();
        services.AddLogging(builder =>
        {
            builder.AddSerilog();
        });

        // 加载并注册模块
        // 注意：这里需要先注册ILogger，所以使用临时服务提供者
        var tempServices = new ServiceCollection();
        tempServices.AddSingleton<IConfiguration>(configuration);
        tempServices.AddLogging();

        // 使用 using 确保临时 ServiceProvider 被正确释放
        using var tempProvider = tempServices.BuildServiceProvider();

        var moduleLoader = new ModuleLoader(
            tempProvider.GetRequiredService<ILogger<ModuleLoader>>(),
            configuration);

        // 异步加载模块
        _loadedModules = await moduleLoader.LoadModulesAsync(cancellationToken);

        // 获取模块契约注册表
        var contractRegistry = new ModuleContractRegistry();

        // 按顺序配置模块服务（使用异步方法）
        foreach (var module in _loadedModules)
        {
            _logger.LogInformation("配置模块服务: {ModuleName}", module.Name);
            await module.PreConfigureServicesAsync(services, configuration, cancellationToken);
            await module.ConfigureServicesAsync(services, configuration, cancellationToken);
        }

        // 后配置（按依赖逆序）
        foreach (var module in _loadedModules.Reverse())
        {
            await module.PostConfigureServicesAsync(services, configuration, cancellationToken);
        }

        // 注册契约注册表
        services.AddSingleton(contractRegistry);

        _logger.LogInformation("已注册 {Count} 个模块", _loadedModules.Count);
    }

    public void ConfigureHost(IHostBuilder hostBuilder)
    {
        hostBuilder
            .UseSerilog((context, services, configuration) =>
            {
                var appConfiguration = context.Configuration;
                var loggerConfig = SerilogConfiguration.ConfigureSerilog(appConfiguration);
                loggerConfig.ReadFrom.Configuration(appConfiguration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
                Log.Logger = loggerConfig.CreateLogger();
            })
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                       .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                       .AddEnvironmentVariables();
            });
    }
    
    private IReadOnlyList<BootstrapError> ValidateConfiguration()
    {
        var errors = new List<BootstrapError>();
        
        // 验证Lemoo配置节
        var lemooSection = _configuration.GetSection("Lemoo");
        if (!lemooSection.Exists())
        {
            errors.Add(new BootstrapError
            {
                Code = "MISSING_CONFIG",
                Message = "缺少 'Lemoo' 配置节"
            });
            return errors;
        }
        
        // 验证模块配置
        var modulesSection = lemooSection.GetSection("Modules");
        if (!modulesSection.Exists())
        {
            errors.Add(new BootstrapError
            {
                Code = "MISSING_MODULES_CONFIG",
                Message = "缺少 'Lemoo:Modules' 配置节"
            });
        }
        
        // 验证数据库配置
        var databaseSection = lemooSection.GetSection("Database");
        if (!databaseSection.Exists())
        {
            errors.Add(new BootstrapError
            {
                Code = "MISSING_DATABASE_CONFIG",
                Message = "缺少 'Lemoo:Database' 配置节"
            });
        }
        
        return errors;
    }
    
}

