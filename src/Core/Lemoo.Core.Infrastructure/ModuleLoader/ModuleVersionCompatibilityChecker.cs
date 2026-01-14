using Lemoo.Core.Abstractions.Module;
using Microsoft.Extensions.Logging;

namespace Lemoo.Core.Infrastructure.ModuleLoader;

/// <summary>
/// 模块版本兼容性检查器
/// </summary>
public class ModuleVersionCompatibilityChecker
{
    private readonly ILogger<ModuleVersionCompatibilityChecker> _logger;

    public ModuleVersionCompatibilityChecker(ILogger<ModuleVersionCompatibilityChecker> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 验证模块依赖的版本兼容性
    /// </summary>
    public ModuleCompatibilityResult ValidateCompatibility(IReadOnlyList<IModule> modules)
    {
        var moduleMap = modules.ToDictionary(m => m.Name, m => m);
        var errors = new List<ModuleCompatibilityError>();
        var warnings = new List<ModuleCompatibilityWarning>();

        foreach (var module in modules)
        {
            // 1. 检查新版本的依赖（如果定义了）
            if (module.DependencyModules.Count > 0)
            {
                ValidateDependencyModules(module, moduleMap, errors, warnings);
            }
            // 2. 兼容旧版本的依赖
            else if (module.Dependencies.Count > 0)
            {
                ValidateLegacyDependencies(module, moduleMap, errors, warnings);
            }
        }

        return new ModuleCompatibilityResult
        {
            IsCompatible = errors.Count == 0,
            Errors = errors,
            Warnings = warnings
        };
    }

    private void ValidateDependencyModules(
        IModule module,
        Dictionary<string, IModule> moduleMap,
        List<ModuleCompatibilityError> errors,
        List<ModuleCompatibilityWarning> warnings)
    {
        foreach (var dependency in module.DependencyModules)
        {
            // 检查依赖模块是否存在
            if (!moduleMap.ContainsKey(dependency.ModuleName))
            {
                if (dependency.IsRequired)
                {
                    errors.Add(new ModuleCompatibilityError
                    {
                        ModuleName = module.Name,
                        DependencyName = dependency.ModuleName,
                        ErrorType = ModuleCompatibilityErrorType.MissingDependency,
                        Message = $"模块 '{module.Name}' 依赖的模块 '{dependency.ModuleName}' 未找到"
                    });
                }
                else
                {
                    warnings.Add(new ModuleCompatibilityWarning
                    {
                        ModuleName = module.Name,
                        DependencyName = dependency.ModuleName,
                        Message = $"模块 '{module.Name}' 的可选依赖 '{dependency.ModuleName}' 未找到"
                    });
                }
                continue;
            }

            var dependencyModule = moduleMap[dependency.ModuleName];

            // 检查版本兼容性
            if (!string.IsNullOrWhiteSpace(dependency.VersionRange))
            {
                var versionRange = dependency.ParseVersionRange();
                var dependencyVersion = new Version(dependencyModule.Version);

                if (!versionRange.IsSatisfied(dependencyVersion))
                {
                    errors.Add(new ModuleCompatibilityError
                    {
                        ModuleName = module.Name,
                        DependencyName = dependency.ModuleName,
                        ErrorType = ModuleCompatibilityErrorType.VersionMismatch,
                        Message = $"模块 '{module.Name}' 需要 '{dependency.ModuleName}' 版本 {dependency.VersionRange}，" +
                                 $"但找到版本 {dependencyModule.Version}",
                        RequiredVersion = dependency.VersionRange,
                        ActualVersion = dependencyModule.Version
                    });
                }
                else
                {
                    _logger.LogDebug(
                        "版本兼容性检查通过: {ModuleName} 需要 {DependencyName} {VersionRange}，找到 {ActualVersion}",
                        module.Name, dependency.ModuleName, dependency.VersionRange, dependencyModule.Version);
                }
            }
        }
    }

    private void ValidateLegacyDependencies(
        IModule module,
        Dictionary<string, IModule> moduleMap,
        List<ModuleCompatibilityError> errors,
        List<ModuleCompatibilityWarning> warnings)
    {
        foreach (var dependencyName in module.Dependencies)
        {
            if (!moduleMap.ContainsKey(dependencyName))
            {
                errors.Add(new ModuleCompatibilityError
                {
                    ModuleName = module.Name,
                    DependencyName = dependencyName,
                    ErrorType = ModuleCompatibilityErrorType.MissingDependency,
                    Message = $"模块 '{module.Name}' 依赖的模块 '{dependencyName}' 未找到"
                });
            }
        }
    }
}

/// <summary>
/// 模块兼容性检查结果
/// </summary>
public class ModuleCompatibilityResult
{
    public bool IsCompatible { get; set; }
    public IReadOnlyList<ModuleCompatibilityError> Errors { get; set; } = Array.Empty<ModuleCompatibilityError>();
    public IReadOnlyList<ModuleCompatibilityWarning> Warnings { get; set; } = Array.Empty<ModuleCompatibilityWarning>();
}

/// <summary>
/// 模块兼容性错误
/// </summary>
public class ModuleCompatibilityError
{
    public string ModuleName { get; set; } = string.Empty;
    public string DependencyName { get; set; } = string.Empty;
    public ModuleCompatibilityErrorType ErrorType { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? RequiredVersion { get; set; }
    public string? ActualVersion { get; set; }
}

/// <summary>
/// 模块兼容性警告
/// </summary>
public class ModuleCompatibilityWarning
{
    public string ModuleName { get; set; } = string.Empty;
    public string DependencyName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 模块兼容性错误类型
/// </summary>
public enum ModuleCompatibilityErrorType
{
    /// <summary>
    /// 缺少依赖
    /// </summary>
    MissingDependency,

    /// <summary>
    /// 版本不匹配
    /// </summary>
    VersionMismatch,

    /// <summary>
    /// 循环依赖
    /// </summary>
    CircularDependency
}
