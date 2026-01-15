using Lemoo.Core.Abstractions.Module;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Infrastructure.ModuleLoader;

/// <summary>
/// 模块健康检查器
/// </summary>
public class ModuleHealthChecker
{
    private readonly IModuleLoader _moduleLoader;
    private readonly IModuleLifecycleManager _lifecycleManager;
    private readonly ModuleMemoryLeakProtector _memoryProtector;
    private readonly ILogger<ModuleHealthChecker> _logger;

    public ModuleHealthChecker(
        IModuleLoader moduleLoader,
        IModuleLifecycleManager lifecycleManager,
        ModuleMemoryLeakProtector memoryProtector,
        ILogger<ModuleHealthChecker> logger)
    {
        _moduleLoader = moduleLoader;
        _lifecycleManager = lifecycleManager;
        _memoryProtector = memoryProtector;
        _logger = logger;
    }

    /// <summary>
    /// 检查所有模块的健康状态
    /// </summary>
    public async Task<ModuleSystemHealthReport> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        var modules = _moduleLoader.GetLoadedModules();
        var moduleHealthReports = new List<ModuleHealthReport>();

        foreach (var module in modules)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var healthReport = await CheckModuleHealthAsync(module.Name, cancellationToken);
            moduleHealthReports.Add(healthReport);
        }

        var overallStatus = DetermineOverallHealth(moduleHealthReports);

        return new ModuleSystemHealthReport
        {
            OverallStatus = overallStatus,
            TotalModules = modules.Count,
            HealthyModules = moduleHealthReports.Count(r => r.Status == ModuleHealthStatus.Healthy),
            UnhealthyModules = moduleHealthReports.Count(r => r.Status == ModuleHealthStatus.Unhealthy),
            DegradedModules = moduleHealthReports.Count(r => r.Status == ModuleHealthStatus.Degraded),
            ModuleReports = moduleHealthReports,
            CheckedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// 检查单个模块的健康状态
    /// </summary>
    public async Task<ModuleHealthReport> CheckModuleHealthAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        var module = _moduleLoader.GetModule(moduleName);
        if (module == null)
        {
            return new ModuleHealthReport
            {
                ModuleName = moduleName,
                Status = ModuleHealthStatus.Unhealthy,
                Issues = new List<ModuleHealthIssue>
                {
                    new ModuleHealthIssue
                    {
                        Severity = HealthIssueSeverity.Critical,
                        Description = "模块未找到"
                    }
                },
                CheckedAt = DateTime.UtcNow
            };
        }

        var issues = new List<ModuleHealthIssue>();
        var checks = new List<ModuleHealthCheck>();

        // 1. 检查模块状态
        var stateCheck = await CheckModuleStateAsync(module);
        checks.Add(stateCheck);
        if (!stateCheck.IsHealthy)
        {
            issues.Add(new ModuleHealthIssue
            {
                Severity = HealthIssueSeverity.Warning,
                Description = stateCheck.Message,
                CheckName = "ModuleState"
            });
        }

        // 2. 检查依赖关系
        var dependencyCheck = CheckModuleDependencies(module);
        checks.Add(dependencyCheck);
        if (!dependencyCheck.IsHealthy)
        {
            issues.Add(new ModuleHealthIssue
            {
                Severity = HealthIssueSeverity.Critical,
                Description = dependencyCheck.Message,
                CheckName = "Dependencies"
            });
        }

        // 3. 检查内存状态
        var memoryCheck = CheckModuleMemory(moduleName);
        checks.Add(memoryCheck);
        if (!memoryCheck.IsHealthy)
        {
            issues.Add(new ModuleHealthIssue
            {
                Severity = HealthIssueSeverity.Warning,
                Description = memoryCheck.Message,
                CheckName = "Memory"
            });
        }

        // 4. 检查版本兼容性
        var versionCheck = CheckModuleVersion(module);
        checks.Add(versionCheck);
        if (!versionCheck.IsHealthy)
        {
            issues.Add(new ModuleHealthIssue
            {
                Severity = HealthIssueSeverity.Warning,
                Description = versionCheck.Message,
                CheckName = "Version"
            });
        }

        var status = DetermineModuleHealth(issues);

        return new ModuleHealthReport
        {
            ModuleName = moduleName,
            Status = status,
            Issues = issues,
            Checks = checks,
            CheckedAt = DateTime.UtcNow
        };
    }

    private async Task<ModuleHealthCheck> CheckModuleStateAsync(IModule module)
    {
        try
        {
            var state = await _lifecycleManager.GetModuleStateAsync(module.Name);

            return state switch
            {
                ModuleState.Started => new ModuleHealthCheck
                {
                    Name = "ModuleState",
                    IsHealthy = true,
                    Message = $"模块状态: {state}"
                },
                ModuleState.Starting or ModuleState.Stopping => new ModuleHealthCheck
                {
                    Name = "ModuleState",
                    IsHealthy = true,
                    Message = $"模块正在转换状态: {state}"
                },
                ModuleState.Error => new ModuleHealthCheck
                {
                    Name = "ModuleState",
                    IsHealthy = false,
                    Message = $"模块处于错误状态"
                },
                _ => new ModuleHealthCheck
                {
                    Name = "ModuleState",
                    IsHealthy = false,
                    Message = $"模块状态异常: {state}"
                }
            };
        }
        catch (Exception ex)
        {
            return new ModuleHealthCheck
            {
                Name = "ModuleState",
                IsHealthy = false,
                Message = $"无法获取模块状态: {ex.Message}"
            };
        }
    }

    private ModuleHealthCheck CheckModuleDependencies(IModule module)
    {
        var modules = _moduleLoader.GetLoadedModules();
        var moduleNames = modules.Select(m => m.Name).ToHashSet();
        var missingDependencies = new List<string>();

        // 检查旧版依赖
        foreach (var dep in module.Dependencies)
        {
            if (!moduleNames.Contains(dep))
            {
                missingDependencies.Add(dep);
            }
        }

        // 检查新版依赖
        foreach (var dep in module.DependencyModules)
        {
            if (dep.IsRequired && !moduleNames.Contains(dep.ModuleName))
            {
                missingDependencies.Add(dep.ModuleName);
            }
        }

        if (missingDependencies.Any())
        {
            return new ModuleHealthCheck
            {
                Name = "Dependencies",
                IsHealthy = false,
                Message = $"缺少依赖: {string.Join(", ", missingDependencies)}"
            };
        }

        return new ModuleHealthCheck
        {
            Name = "Dependencies",
            IsHealthy = true,
            Message = "所有依赖都已满足"
        };
    }

    private ModuleHealthCheck CheckModuleMemory(string moduleName)
    {
        try
        {
            var memoryInfo = _memoryProtector.GetModuleMemoryInfo(moduleName);

            if (!memoryInfo.CanBeSafelyUnloaded)
            {
                return new ModuleHealthCheck
                {
                    Name = "Memory",
                    IsHealthy = false,
                    Message = $"模块可能存在内存泄漏: {memoryInfo.AliveReferences}/{memoryInfo.TrackedReferences} 引用仍然存活"
                };
            }

            return new ModuleHealthCheck
            {
                Name = "Memory",
                IsHealthy = true,
                Message = $"内存状态正常: {memoryInfo.TrackedReferences} 跟踪引用"
            };
        }
        catch (Exception ex)
        {
            return new ModuleHealthCheck
            {
                Name = "Memory",
                IsHealthy = false,
                Message = $"内存检查失败: {ex.Message}"
            };
        }
    }

    private ModuleHealthCheck CheckModuleVersion(IModule module)
    {
        try
        {
            var version = new Version(module.Version);

            // 检查版本是否有效
            if (version.Major == 0 && version.Minor == 0 && version.Build == 0)
            {
                return new ModuleHealthCheck
                {
                    Name = "Version",
                    IsHealthy = false,
                    Message = $"模块版本无效: {module.Version}"
                };
            }

            return new ModuleHealthCheck
            {
                Name = "Version",
                IsHealthy = true,
                Message = $"版本 {module.Version}"
            };
        }
        catch (Exception ex)
        {
            return new ModuleHealthCheck
            {
                Name = "Version",
                IsHealthy = false,
                Message = $"版本解析失败: {ex.Message}"
            };
        }
    }

    private ModuleHealthStatus DetermineModuleHealth(List<ModuleHealthIssue> issues)
    {
        if (!issues.Any())
        {
            return ModuleHealthStatus.Healthy;
        }

        var hasCritical = issues.Any(i => i.Severity == HealthIssueSeverity.Critical);
        if (hasCritical)
        {
            return ModuleHealthStatus.Unhealthy;
        }

        return ModuleHealthStatus.Degraded;
    }

    private ModuleSystemHealthStatus DetermineOverallHealth(List<ModuleHealthReport> reports)
    {
        if (!reports.Any())
        {
            return ModuleSystemHealthStatus.NoModules;
        }

        var hasUnhealthy = reports.Any(r => r.Status == ModuleHealthStatus.Unhealthy);
        if (hasUnhealthy)
        {
            return ModuleSystemHealthStatus.Unhealthy;
        }

        var hasDegraded = reports.Any(r => r.Status == ModuleHealthStatus.Degraded);
        if (hasDegraded)
        {
            return ModuleSystemHealthStatus.Degraded;
        }

        return ModuleSystemHealthStatus.Healthy;
    }

    /// <summary>
    /// 获取模块诊断信息
    /// </summary>
    public async Task<ModuleDiagnostics> GetDiagnosticsAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        var module = _moduleLoader.GetModule(moduleName);
        if (module == null)
        {
            throw new InvalidOperationException($"模块未找到: {moduleName}");
        }

        var state = await _lifecycleManager.GetModuleStateAsync(moduleName);
        var memoryInfo = _memoryProtector.GetModuleMemoryInfo(moduleName);
        var healthReport = await CheckModuleHealthAsync(moduleName, cancellationToken);

        return new ModuleDiagnostics
        {
            ModuleName = moduleName,
            Version = module.Version,
            Description = module.Description,
            State = state,
            HealthStatus = healthReport.Status,
            MemoryInfo = memoryInfo,
            Dependencies = module.Dependencies.ToList(),
            DependencyModules = module.DependencyModules.Select(d => new ModuleDependencyInfo
            {
                ModuleName = d.ModuleName,
                VersionRange = d.VersionRange,
                IsRequired = d.IsRequired
            }).ToList(),
            Metadata = module.Metadata,
            DiagnosedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// 模块系统健康报告
/// </summary>
public class ModuleSystemHealthReport
{
    public ModuleSystemHealthStatus OverallStatus { get; set; }
    public int TotalModules { get; set; }
    public int HealthyModules { get; set; }
    public int UnhealthyModules { get; set; }
    public int DegradedModules { get; set; }
    public List<ModuleHealthReport> ModuleReports { get; set; } = new();
    public DateTime CheckedAt { get; set; }

    public override string ToString()
    {
        return $"模块系统健康: {OverallStatus} ({HealthyModules}/{TotalModules} 健康)";
    }
}

/// <summary>
/// 模块健康报告
/// </summary>
public class ModuleHealthReport
{
    public string ModuleName { get; set; } = string.Empty;
    public ModuleHealthStatus Status { get; set; }
    public List<ModuleHealthIssue> Issues { get; set; } = new();
    public List<ModuleHealthCheck> Checks { get; set; } = new();
    public DateTime CheckedAt { get; set; }
}

/// <summary>
/// 模块健康问题
/// </summary>
public class ModuleHealthIssue
{
    public HealthIssueSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? CheckName { get; set; }
}

/// <summary>
/// 模块健康检查
/// </summary>
public class ModuleHealthCheck
{
    public string Name { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 模块诊断信息
/// </summary>
public class ModuleDiagnostics
{
    public string ModuleName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ModuleState State { get; set; }
    public ModuleHealthStatus HealthStatus { get; set; }
    public ModuleMemoryInfo MemoryInfo { get; set; } = null!;
    public List<string> Dependencies { get; set; } = new();
    public List<ModuleDependencyInfo> DependencyModules { get; set; } = new();
    public ModuleMetadata Metadata { get; set; } = null!;
    public DateTime DiagnosedAt { get; set; }
}

/// <summary>
/// 模块依赖信息
/// </summary>
public class ModuleDependencyInfo
{
    public string ModuleName { get; set; } = string.Empty;
    public string VersionRange { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
}

/// <summary>
/// 模块健康状态
/// </summary>
public enum ModuleHealthStatus
{
    /// <summary>
    /// 健康
    /// </summary>
    Healthy,

    /// <summary>
    /// 降级
    /// </summary>
    Degraded,

    /// <summary>
    /// 不健康
    /// </summary>
    Unhealthy
}

/// <summary>
/// 模块系统健康状态
/// </summary>
public enum ModuleSystemHealthStatus
{
    /// <summary>
    /// 无模块
    /// </summary>
    NoModules,

    /// <summary>
    /// 健康
    /// </summary>
    Healthy,

    /// <summary>
    /// 降级
    /// </summary>
    Degraded,

    /// <summary>
    /// 不健康
    /// </summary>
    Unhealthy
}

/// <summary>
/// 健康问题严重程度
/// </summary>
public enum HealthIssueSeverity
{
    /// <summary>
    /// 信息
    /// </summary>
    Info,

    /// <summary>
    /// 警告
    /// </summary>
    Warning,

    /// <summary>
    /// 严重
    /// </summary>
    Critical
}
