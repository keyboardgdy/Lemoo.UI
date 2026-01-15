using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lemoo.UI.Controls;
using Lemoo.UI.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Lemoo.UI.WPF.ViewModels.Pages;

/// <summary>
/// 工具箱示例页面视图模型
/// </summary>
public partial class ToolboxSampleViewModel : ObservableObject, IDisposable
{
    /// <summary>
    /// 工具箱中选中的控件
    /// </summary>
    [ObservableProperty]
    private ControlInfo? _selectedControl;

    /// <summary>
    /// 选中的控件详细信息
    /// </summary>
    [ObservableProperty]
    private string? _controlDetails;

    /// <summary>
    /// 示例代码
    /// </summary>
    [ObservableProperty]
    private string _sampleCode = string.Empty;

    /// <summary>
    /// 选中的样式变体
    /// </summary>
    [ObservableProperty]
    private ControlStyleVariant? _selectedStyleVariant;

    /// <summary>
    /// 控件样式变体集合
    /// </summary>
    public ObservableCollection<ControlStyleVariant> StyleVariants { get; }

    /// <summary>
    /// 预览控件
    /// </summary>
    [ObservableProperty]
    private UIElement? _previewControl;

    public ToolboxSampleViewModel()
    {
        StyleVariants = new ObservableCollection<ControlStyleVariant>();
    }

    /// <summary>
    /// 当工具箱选中控件变化时调用
    /// </summary>
    partial void OnSelectedControlChanged(ControlInfo? value)
    {
        if (value == null)
        {
            ControlDetails = null;
            SampleCode = string.Empty;
            StyleVariants.Clear();
            SelectedStyleVariant = null;
            PreviewControl = null;
            return;
        }

        // 更新控件详细信息
        ControlDetails = $"{value.DisplayName} ({value.Name})\n{value.Description}";

        // 更新示例代码
        SampleCode = value.SampleCode;

        // 更新样式变体
        StyleVariants.Clear();
        if (value.StyleVariants != null)
        {
            foreach (var variant in value.StyleVariants)
            {
                StyleVariants.Add(variant);
            }
            SelectedStyleVariant = StyleVariants.FirstOrDefault();
        }
        else
        {
            SelectedStyleVariant = null;
        }

        // 生成预览控件
        UpdatePreviewControl();
    }

    /// <summary>
    /// 当选中样式变体变化时调用
    /// </summary>
    partial void OnSelectedStyleVariantChanged(ControlStyleVariant? value)
    {
        if (value == null || SelectedControl == null)
        {
            return;
        }

        // 更新示例代码为样式变体版本
        if (!string.IsNullOrEmpty(value.StyleKey))
        {
            SampleCode = $"<{SelectedControl.Name} Style=\"{{StaticResource {value.StyleKey}}}\" Content=\"{SelectedControl.DisplayName}\" />";
        }
        else
        {
            SampleCode = SelectedControl.SampleCode;
        }

        // 更新预览控件
        UpdatePreviewControl();
    }

    /// <summary>
    /// 复制示例代码到剪贴板
    /// </summary>
    [RelayCommand]
    private void CopySampleCode()
    {
        if (!string.IsNullOrEmpty(SampleCode))
        {
            System.Windows.Clipboard.SetText(SampleCode);
        }
    }

    /// <summary>
    /// 选择样式变体
    /// </summary>
    [RelayCommand]
    private void SelectStyleVariant(ControlStyleVariant variant)
    {
        SelectedStyleVariant = variant;
    }

    /// <summary>
    /// 更新预览控件
    /// </summary>
    private void UpdatePreviewControl()
    {
        if (SelectedControl == null)
        {
            PreviewControl = null;
            return;
        }

        try
        {
            var styleKey = SelectedStyleVariant?.StyleKey;
            PreviewControl = CreatePreviewControl(SelectedControl.Name, styleKey);
        }
        catch
        {
            // 如果创建失败，创建一个文本提示
            var errorText = new TextBlock
            {
                Text = "预览不可用",
                Foreground = System.Windows.Media.Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            PreviewControl = errorText;
        }
    }

    /// <summary>
    /// 创建预览控件
    /// </summary>
    private UIElement CreatePreviewControl(string controlName, string? styleKey)
    {
        return controlName switch
        {
            "Button" => CreateButtonPreview(styleKey),
            "ToggleButton" => CreateToggleButtonPreview(styleKey),
            "DropDownButton" => CreateDropDownButtonPreview(styleKey),
            "SplitButton" => CreateSplitButtonPreview(styleKey),
            "Badge" => CreateBadgePreview(styleKey),
            "TextBox" => CreateTextBoxPreview(styleKey),
            "NumericUpDown" => CreateNumericUpDownPreview(styleKey),
            "SearchBox" => CreateSearchBoxPreview(styleKey),
            "CheckBox" => CreateCheckBoxPreview(styleKey),
            "ToggleSwitch" => CreateToggleSwitchPreview(styleKey),
            "ProgressBar" => CreateProgressBarPreview(styleKey),
            "ProgressRing" => CreateProgressRingPreview(styleKey),
            "Expander" => CreateExpanderPreview(styleKey),
            _ => CreateDefaultPreview(controlName, styleKey)
        };
    }

    private Button CreateButtonPreview(string? styleKey)
    {
        var button = new Button { Content = "按钮示例", Margin = new Thickness(8) };
        ApplyStyle(button, styleKey ?? "Win11.Button");
        return button;
    }

    private ToggleButton CreateToggleButtonPreview(string? styleKey)
    {
        var button = new ToggleButton { Content = "切换按钮", Margin = new Thickness(8), IsChecked = true };
        ApplyStyle(button, styleKey ?? "Win11.ToggleButton");
        return button;
    }

    private Button CreateDropDownButtonPreview(string? styleKey)
    {
        var button = new Button
        {
            Content = "下拉按钮",
            Margin = new Thickness(8)
        };
        ApplyStyle(button, styleKey ?? "Win11.Button.DropDown");
        return button;
    }

    private Border CreateSplitButtonPreview(string? styleKey)
    {
        // SplitButton 需要特殊处理，这里用简单的 Border 模拟
        return new Border
        {
            Background = System.Windows.Media.Brushes.LightGray,
            Width = 120,
            Height = 32,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(8),
            Child = new TextBlock
            {
                Text = "分割按钮",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
    }

    private Badge CreateBadgePreview(string? styleKey)
    {
        return new Badge
        {
            Content = "99+",
            Margin = new Thickness(8),
            HorizontalAlignment = HorizontalAlignment.Left
        };
    }

    private TextBox CreateTextBoxPreview(string? styleKey)
    {
        var textBox = new TextBox
        {
            Text = "文本输入示例",
            Width = 200,
            Margin = new Thickness(8)
        };
        ApplyStyle(textBox, styleKey ?? "Win11.TextBox");
        return textBox;
    }

    private Controls.NumericUpDown CreateNumericUpDownPreview(string? styleKey)
    {
        var nud = new Controls.NumericUpDown
        {
            Value = 42,
            Width = 120,
            Margin = new Thickness(8),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        return nud;
    }

    private TextBox CreateSearchBoxPreview(string? styleKey)
    {
        var textBox = new TextBox
        {
            Text = "搜索...",
            Width = 200,
            Margin = new Thickness(8)
        };
        ApplyStyle(textBox, "Win11.TextBox.Search");
        return textBox;
    }

    private CheckBox CreateCheckBoxPreview(string? styleKey)
    {
        var checkBox = new CheckBox
        {
            Content = "复选框示例",
            IsChecked = true,
            Margin = new Thickness(8)
        };
        return checkBox;
    }

    private ToggleSwitch CreateToggleSwitchPreview(string? styleKey)
    {
        return new ToggleSwitch
        {
            IsChecked = true,
            Margin = new Thickness(8),
            HorizontalAlignment = HorizontalAlignment.Left
        };
    }

    private ProgressBar CreateProgressBarPreview(string? styleKey)
    {
        return new ProgressBar
        {
            Value = 70,
            Width = 200,
            Height = 4,
            Margin = new Thickness(8),
            HorizontalAlignment = HorizontalAlignment.Left
        };
    }

    private ProgressRing CreateProgressRingPreview(string? styleKey)
    {
        return new ProgressRing
        {
            IsIndeterminate = true,
            Width = 40,
            Height = 40,
            Margin = new Thickness(8),
            HorizontalAlignment = HorizontalAlignment.Left
        };
    }

    private Controls.Expander CreateExpanderPreview(string? styleKey)
    {
        return new Controls.Expander
        {
            Header = "展开器示例",
            Content = new TextBlock { Text = "这是展开器的内容", Margin = new Thickness(8) },
            IsExpanded = true,
            Margin = new Thickness(8)
        };
    }

    private Border CreateDefaultPreview(string controlName, string? styleKey)
    {
        return new Border
        {
            Background = System.Windows.Media.Brushes.Transparent,
            BorderBrush = System.Windows.Media.Brushes.LightGray,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(16),
            Child = new StackPanel
            {
                Children =
                {
                    new TextBlock
                    {
                        Text = $"{controlName}",
                        FontWeight = System.Windows.FontWeights.SemiBold,
                        Margin = new Thickness(0, 0, 0, 4)
                    },
                    new TextBlock
                    {
                        Text = styleKey ?? "默认样式",
                        FontSize = 11,
                        Foreground = System.Windows.Media.Brushes.Gray
                    }
                }
            }
        };
    }

    private void ApplyStyle(FrameworkElement element, string styleKey)
    {
        try
        {
            if (Application.Current?.TryFindResource(styleKey) is Style style)
            {
                element.Style = style;
            }
        }
        catch
        {
            // 忽略样式应用失败
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        StyleVariants.Clear();
    }
}
