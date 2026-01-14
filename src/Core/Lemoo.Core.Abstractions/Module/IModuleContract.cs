namespace Lemoo.Core.Abstractions.Module;

/// <summary>
/// 模块契约接口 - 定义模块对外提供的服务接口
/// </summary>
public interface IModuleContract
{
    /// <summary>
    /// 契约名称（唯一标识）
    /// </summary>
    string ContractName { get; }

    /// <summary>
    /// 契约版本
    /// </summary>
    string Version { get; }

    /// <summary>
    /// 契约描述
    /// </summary>
    string Description { get; }

    /// <summary>
    /// 提供此契约的模块名称
    /// </summary>
    string ProviderModule { get; }
}

/// <summary>
/// 模块契约注册表接口
/// </summary>
public interface IModuleContractRegistry
{
    /// <summary>
    /// 注册契约
    /// </summary>
    void RegisterContract(IModuleContract contract, object implementation);

    /// <summary>
    /// 获取契约实现
    /// </summary>
    T? GetContract<T>(string contractName) where T : class;

    /// <summary>
    /// 获取契约实现
    /// </summary>
    object? GetContract(string contractName);

    /// <summary>
    /// 获取契约实现（按类型）
    /// </summary>
    T? GetContract<T>() where T : class, IModuleContract;

    /// <summary>
    /// 检查契约是否已注册
    /// </summary>
    bool IsRegistered(string contractName);

    /// <summary>
    /// 获取所有注册的契约
    /// </summary>
    IReadOnlyDictionary<string, IModuleContract> GetRegisteredContracts();

    /// <summary>
    /// 注销契约
    /// </summary>
    bool UnregisterContract(string contractName);
}

/// <summary>
/// 模块契约注册表实现
/// </summary>
public class ModuleContractRegistry : IModuleContractRegistry
{
    private readonly Dictionary<string, (IModuleContract contract, object implementation)> _contracts = new();

    public void RegisterContract(IModuleContract contract, object implementation)
    {
        if (contract == null)
            throw new ArgumentNullException(nameof(contract));

        if (implementation == null)
            throw new ArgumentNullException(nameof(implementation));

        var contractName = contract.ContractName;

        if (_contracts.ContainsKey(contractName))
        {
            throw new InvalidOperationException($"契约 '{contractName}' 已被注册");
        }

        _contracts[contractName] = (contract, implementation);
    }

    public T? GetContract<T>(string contractName) where T : class
    {
        if (_contracts.TryGetValue(contractName, out var entry))
        {
            return entry.implementation as T;
        }
        return null;
    }

    public object? GetContract(string contractName)
    {
        if (_contracts.TryGetValue(contractName, out var entry))
        {
            return entry.implementation;
        }
        return null;
    }

    public T? GetContract<T>() where T : class, IModuleContract
    {
        var contractName = typeof(T).Name;
        if (_contracts.TryGetValue(contractName, out var entry))
        {
            return entry.implementation as T;
        }
        return null;
    }

    public bool IsRegistered(string contractName)
    {
        return _contracts.ContainsKey(contractName);
    }

    public IReadOnlyDictionary<string, IModuleContract> GetRegisteredContracts()
    {
        return _contracts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.contract);
    }

    public bool UnregisterContract(string contractName)
    {
        return _contracts.Remove(contractName);
    }
}

/// <summary>
/// 模块契约特性 - 用于标记契约接口
/// </summary>
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public class ModuleContractAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }

    public ModuleContractAttribute(string name, string description = "")
    {
        Name = name;
        Description = description;
    }
}

/// <summary>
/// 服务契约特性 - 用于标记服务实现
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ServiceContractAttribute : Attribute
{
    public string ContractName { get; }
    public string ContractVersion { get; }

    public ServiceContractAttribute(string contractName, string contractVersion = "1.0.0")
    {
        ContractName = contractName;
        ContractVersion = contractVersion;
    }
}
