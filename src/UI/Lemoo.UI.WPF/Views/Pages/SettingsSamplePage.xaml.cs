using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Lemoo.UI.Helpers;

namespace Lemoo.UI.WPF.Views.Pages;

/// <summary>
/// 主题设置页面 - 仅保留主题切换功能
/// </summary>
public partial class SettingsSamplePage : Page, INotifyPropertyChanged
{
    private bool _isBaseThemeSelected;
    private bool _isDarkThemeSelected;
    private bool _isLightThemeSelected;

    public SettingsSamplePage()
    {
        InitializeComponent();
        DataContext = this;
        
        // 初始化当前主题状态
        UpdateThemeSelection();
        
        // 监听主题变化事件
        ThemeManager.ThemeChanged += OnThemeChanged;
    }

    /// <summary>
    /// 原色模式是否选中
    /// </summary>
    public bool IsBaseThemeSelected
    {
        get => _isBaseThemeSelected;
        set
        {
            if (_isBaseThemeSelected != value)
            {
                _isBaseThemeSelected = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 深色模式是否选中
    /// </summary>
    public bool IsDarkThemeSelected
    {
        get => _isDarkThemeSelected;
        set
        {
            if (_isDarkThemeSelected != value)
            {
                _isDarkThemeSelected = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 浅色模式是否选中
    /// </summary>
    public bool IsLightThemeSelected
    {
        get => _isLightThemeSelected;
        set
        {
            if (_isLightThemeSelected != value)
            {
                _isLightThemeSelected = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 更新主题选择状态
    /// </summary>
    private void UpdateThemeSelection()
    {
        var currentTheme = ThemeManager.CurrentTheme;
        
        IsBaseThemeSelected = currentTheme == ThemeManager.Theme.Base;
        IsDarkThemeSelected = currentTheme == ThemeManager.Theme.Dark;
        IsLightThemeSelected = currentTheme == ThemeManager.Theme.Light;
    }

    /// <summary>
    /// 主题变化事件处理
    /// </summary>
    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        // 在 UI 线程上更新选择状态
        Dispatcher.Invoke(() =>
        {
            UpdateThemeSelection();
        });
    }

    /// <summary>
    /// 选择原色模式
    /// </summary>
    private void OnBaseThemeSelected(object sender, MouseButtonEventArgs e)
    {
        ThemeManager.CurrentTheme = ThemeManager.Theme.Base;
    }

    /// <summary>
    /// 选择深色模式
    /// </summary>
    private void OnDarkThemeSelected(object sender, MouseButtonEventArgs e)
    {
        ThemeManager.CurrentTheme = ThemeManager.Theme.Dark;
    }

    /// <summary>
    /// 选择浅色模式
    /// </summary>
    private void OnLightThemeSelected(object sender, MouseButtonEventArgs e)
    {
        ThemeManager.CurrentTheme = ThemeManager.Theme.Light;
    }

    /// <summary>
    /// 原色模式 RadioButton 选中事件
    /// </summary>
    private void OnBaseThemeRadioButtonChecked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.IsChecked == true)
        {
            ThemeManager.CurrentTheme = ThemeManager.Theme.Base;
        }
    }

    /// <summary>
    /// 深色模式 RadioButton 选中事件
    /// </summary>
    private void OnDarkThemeRadioButtonChecked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.IsChecked == true)
        {
            ThemeManager.CurrentTheme = ThemeManager.Theme.Dark;
        }
    }

    /// <summary>
    /// 浅色模式 RadioButton 选中事件
    /// </summary>
    private void OnLightThemeRadioButtonChecked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.IsChecked == true)
        {
            ThemeManager.CurrentTheme = ThemeManager.Theme.Light;
        }
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}

/// <summary>
/// 布尔值到边框画刷的转换器（选中时显示浅蓝色边框）
/// </summary>
public class BoolToBorderBrushConverter : System.Windows.Data.IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            // 选中时使用浅蓝色边框 (#4FC3F7 或类似的浅蓝色)
            return new SolidColorBrush(Color.FromRgb(79, 195, 247)); // #4FC3F7 - 浅蓝色
        }
        
        // 未选中时使用普通边框
        return Application.Current.TryFindResource("InputBorderBrush") as Brush 
               ?? new SolidColorBrush(Color.FromRgb(58, 63, 70));
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
