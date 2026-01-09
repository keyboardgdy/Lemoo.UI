namespace Lemoo.Core.Common.Exceptions;

/// <summary>
/// 模块异常
/// </summary>
public class ModuleException : BusinessException
{
    public string ModuleName { get; }
    
    public ModuleException(string moduleName, string message) 
        : base(message)
    {
        ModuleName = moduleName;
    }
    
    public ModuleException(string moduleName, string message, Exception innerException) 
        : base(message, innerException)
    {
        ModuleName = moduleName;
    }
}

/// <summary>
/// 模块依赖异常
/// </summary>
public class ModuleDependencyException : ModuleException
{
    public ModuleDependencyException(string moduleName, string message) 
        : base(moduleName, message)
    {
    }
}

/// <summary>
/// 循环依赖异常
/// </summary>
public class CircularDependencyException : ModuleException
{
    public CircularDependencyException(string moduleName, string message) 
        : base(moduleName, message)
    {
    }
}

