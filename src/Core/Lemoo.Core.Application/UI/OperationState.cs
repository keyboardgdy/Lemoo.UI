using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Lemoo.Core.Application.UI;

/// <summary>
/// 操作状态类 - 用于WPF UI状态管理，支持数据绑定
/// </summary>
public class OperationState : INotifyPropertyChanged
{
    private bool _isLoading;
    private bool _hasError;
    private string? _errorMessage;
    private bool _isSuccess;
    private string? _successMessage;
    private double? _progress;
    private string? _statusMessage;

    /// <summary>
    /// 是否正在加载
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>
    /// 是否有错误
    /// </summary>
    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            SetProperty(ref _errorMessage, value);
            HasError = !string.IsNullOrWhiteSpace(value);
        }
    }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess
    {
        get => _isSuccess;
        set => SetProperty(ref _isSuccess, value);
    }

    /// <summary>
    /// 成功消息
    /// </summary>
    public string? SuccessMessage
    {
        get => _successMessage;
        set
        {
            SetProperty(ref _successMessage, value);
            IsSuccess = !string.IsNullOrWhiteSpace(value);
        }
    }

    /// <summary>
    /// 进度（0-100）
    /// </summary>
    public double? Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }

    /// <summary>
    /// 状态消息
    /// </summary>
    public string? StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    /// <summary>
    /// 重置状态
    /// </summary>
    public void Reset()
    {
        IsLoading = false;
        HasError = false;
        ErrorMessage = null;
        IsSuccess = false;
        SuccessMessage = null;
        Progress = null;
        StatusMessage = null;
    }

    /// <summary>
    /// 设置为加载状态
    /// </summary>
    public void SetLoading(string? statusMessage = null)
    {
        Reset();
        IsLoading = true;
        StatusMessage = statusMessage;
    }

    /// <summary>
    /// 设置为成功状态
    /// </summary>
    public void SetSuccess(string? message = null)
    {
        IsLoading = false;
        HasError = false;
        ErrorMessage = null;
        IsSuccess = true;
        SuccessMessage = message;
    }

    /// <summary>
    /// 设置为错误状态
    /// </summary>
    public void SetError(string? message)
    {
        IsLoading = false;
        HasError = true;
        ErrorMessage = message;
        IsSuccess = false;
        SuccessMessage = null;
    }
    
    /// <summary>
    /// 更新进度
    /// </summary>
    public void UpdateProgress(double progress, string? statusMessage = null)
    {
        if (progress < 0)
            progress = 0;
        if (progress > 100)
            progress = 100;
            
        Progress = progress;
        if (!string.IsNullOrWhiteSpace(statusMessage))
            StatusMessage = statusMessage;
    }
    
    /// <summary>
    /// 设置加载状态并更新进度
    /// </summary>
    public void SetLoadingWithProgress(double progress, string? statusMessage = null)
    {
        SetLoading(statusMessage);
        UpdateProgress(progress, statusMessage);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

