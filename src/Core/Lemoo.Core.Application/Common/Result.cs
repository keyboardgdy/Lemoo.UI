namespace Lemoo.Core.Application.Common;

/// <summary>
/// 结果类 - 用于表示操作结果
/// </summary>
public class Result
{
    public bool IsSuccess { get; protected set; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; protected set; }
    public IReadOnlyList<string> Errors { get; protected set; } = Array.Empty<string>();
    
    protected Result(bool isSuccess, string? error = null, IEnumerable<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors != null ? errors.ToList().AsReadOnly() : Array.Empty<string>();
    }
    
    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(IEnumerable<string> errors) => new(false, null, errors);
    
    public static Result<T> Success<T>(T data) => new(true, data);
    public static Result<T> Failure<T>(string error) => new(false, default, error);
    public static Result<T> Failure<T>(IEnumerable<string> errors) => new(false, default, null, errors);
    
    /// <summary>
    /// 成功时执行操作
    /// </summary>
    public Result OnSuccess(Action action)
    {
        if (IsSuccess)
            action();
        return this;
    }
    
    /// <summary>
    /// 失败时执行操作
    /// </summary>
    public Result OnFailure(Action<string?> action)
    {
        if (IsFailure)
            action(Error);
        return this;
    }
    
    /// <summary>
    /// 模式匹配
    /// </summary>
    public TResult Match<TResult>(Func<TResult> onSuccess, Func<string?, TResult> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(Error);
    }
}

/// <summary>
/// 带数据的结果类
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class Result<T> : Result
{
    public T? Data { get; private set; }
    
    internal Result(bool isSuccess, T? data = default, string? error = null, IEnumerable<string>? errors = null)
        : base(isSuccess, error, errors)
    {
        Data = data;
    }
    
    public static Result<T> Success(T data) => new(true, data);
    public static new Result<T> Failure(string error) => new(false, default, error);
    public static new Result<T> Failure(IEnumerable<string> errors) => new(false, default, null, errors);
    
    /// <summary>
    /// 映射结果值
    /// </summary>
    public Result<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        if (IsFailure)
            return Result<TResult>.Failure(Error ?? string.Join("; ", Errors));
        
        if (Data == null)
            return Result<TResult>.Failure("数据为空");
        
        try
        {
            return Result<TResult>.Success(mapper(Data));
        }
        catch (Exception ex)
        {
            return Result<TResult>.Failure($"映射失败: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 绑定结果（链式操作）
    /// </summary>
    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
    {
        if (IsFailure)
            return Result<TResult>.Failure(Error ?? string.Join("; ", Errors));
        
        if (Data == null)
            return Result<TResult>.Failure("数据为空");
        
        try
        {
            return binder(Data);
        }
        catch (Exception ex)
        {
            return Result<TResult>.Failure($"绑定失败: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 模式匹配
    /// </summary>
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<IReadOnlyList<string>, TResult> onFailure)
    {
        return IsSuccess && Data != null ? onSuccess(Data) : onFailure(Errors);
    }
    
    /// <summary>
    /// 成功时执行操作
    /// </summary>
    public Result<T> OnSuccess(Action<T> action)
    {
        if (IsSuccess && Data != null)
            action(Data);
        return this;
    }
    
    /// <summary>
    /// 失败时执行操作
    /// </summary>
    public new Result<T> OnFailure(Action<string?> action)
    {
        if (IsFailure)
            action(Error);
        return this;
    }
    
    /// <summary>
    /// 如果失败，返回默认值
    /// </summary>
    public T ValueOr(T defaultValue)
    {
        return IsSuccess && Data != null ? Data : defaultValue;
    }
    
    /// <summary>
    /// 如果失败，返回默认值（通过函数计算）
    /// </summary>
    public T ValueOr(Func<T> defaultValueFactory)
    {
        return IsSuccess && Data != null ? Data : defaultValueFactory();
    }
}

