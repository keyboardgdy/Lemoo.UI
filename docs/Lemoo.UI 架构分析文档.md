# Lemoo.UI 架构分析文档

## 一、项目概述

### 1.1 基本信息

| 项目信息 | 内容 |
|---------|------|
| **项目名称** | Lemoo.UI |
| **版本** | 1.0.0 |
| **目标框架** | .NET 10.0-windows |
| **开发语言** | C# / XAML |
| **作者** | Lemoo Team |
| **描述** | 专业级 WPF UI 组件库 |

### 1.2 项目定位

Lemoo.UI 是一个**专业级的 WPF UI 组件库**，具有以下特点：

- **现代化设计语言**：采用 Windows 11 Fluent Design 风格
- **多主题支持**：内置 6 种主题，支持运行时切换
- **模块化架构**：支持插件式模块加载和 UI 组件注册
- **完整控件集**：涵盖按钮、输入、布局、对话框、通知等所有常见场景
- **MVVM 友好**：深度集成 CommunityToolkit.Mvvm
- **设计时支持**：完整的工具箱集成和设计时数据支持

---

## 二、架构设计

### 2.1 整体架构图

```
┌─────────────────────────────────────────────────────────────┐
│                     应用层 (WPF Application)                 │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                   Lemoo.UI.WPF (示例应用)                    │
│  - MainWindow                                               │
│  - PageViewModels                                           │
│  - Navigation                                               │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                      Lemoo.UI (控件库)                       │
├─────────────────────────────────────────────────────────────┤
│  控件层              │  样式层          │  主题层           │
│  - Buttons           │  - Design        │  - Base           │
│  - Inputs            │  - Win11         │  - Dark           │
│  - Layout            │  - Common        │  - Light          │
│  - Dialogs           │                  │  - NeonCyberpunk  │
│  - Notifications     │                  │  - Aurora         │
│  - Navigation        │                  │  - SunsetTropics  │
├─────────────────────┴──────────────────┴─────────────────────┤
│  服务层                        │  抽象层                      │
│  - ControlRegistry             │  - IPageRegistry            │
│  - ThemeManager                │  - IModuleUI                │
├────────────────────────────────┴─────────────────────────────┤
│  基础设施层                                                    │
│  - Behaviors (行为)                                           │
│  - Commands (命令)                                            │
│  - Converters (转换器)                                        │
│  - Interactivity (交互)                                       │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│              Lemoo.Core.Infrastructure (基础设施)             │
│  - ModuleLoader (模块加载器)                                  │
│  - Caching (缓存)                                            │
│  - Persistence (持久化)                                      │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 分层架构

#### 2.2.1 控件层 (Controls)
提供所有自定义控件的实现，按功能分类组织：

| 分类 | 控件 |
|------|------|
| **Buttons** | Badge, DropDownButton, ToggleSwitch |
| **Inputs** | AutoCompleteBox, NumericUpDown, SearchBox, TimePicker |
| **Cards** | Card, Expander |
| **Dialogs** | DialogHost, MessageBox |
| **Notifications** | Snackbar |
| **Progress** | ProgressRing |
| **Layout** | Ribbon, SplitView |
| **Navigation** | Sidebar, DocumentTabHost |
| **Chrome** | MainTitleBar |
| **Toolbox** | ToolboxView |

#### 2.2.2 样式层 (Styles)
提供与主题解耦的设计系统和 Win11 风格控件样式：

- **Design 目录**：字体、间距、阴影、动画
- **Win11 目录**：所有控件的 Win11 风格样式
- **CommonStyles.xaml**：样式系统主入口

#### 2.2.3 主题层 (Themes)
提供多主题支持，每个主题包含：

- **ColorPalette.xaml**：调色板（基础颜色）
- **SemanticTokens.xaml**：语义令牌（用途颜色）
- **ComponentBrushes.xaml**：组件画刷（实际使用的 Brush）
- **[ThemeName].xaml**：主题入口文件

#### 2.2.4 服务层 (Services)
提供核心服务功能：

- **ControlRegistry**：控件元数据注册表
- **ThemeManager**：主题管理器（核心）

#### 2.2.5 抽象层 (Abstractions)
定义模块化 UI 的接口：

- **IPageRegistry**：页面注册表接口
- **IModuleUI**：模块 UI 接口
- **NavigationItemMetadata**：导航项元数据

---

## 三、核心组件分析

### 3.1 主题系统 (ThemeManager)

#### 3.1.1 设计理念
采用 **VS Code 风格的主题切换方案**，具有以下特点：

1. **资源字典缓存**：避免重复创建，提升性能
2. **异步窗口刷新**：使用低优先级异步刷新，避免阻塞 UI
3. **主题回退机制**：加载失败时自动回退到 Base 主题
4. **事件驱动**：提供 ThemeChanged 事件

#### 3.1.2 架构设计

```csharp
// 主题枚举
public enum Theme
{
    Base,           // 原色模式（默认）
    Dark,           // 深色模式（VS Dark 风格）
    Light,          // 浅色模式（VS Light 风格）
    NeonCyberpunk,  // 霓虹赛博朋克
    Aurora,         // 极光
    SunsetTropics,  // 热带夕阳
    System          // 跟随系统
}

// 核心属性
public static Theme CurrentTheme { get; set; }
public static ReadOnlyCollection<ThemeInfo> AvailableThemes { get; }
public static event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
```

#### 3.1.3 性能优化

| 优化点 | 实现方式 | 效果 |
|--------|---------|------|
| **资源字典缓存** | `Dictionary<Theme, ResourceDictionary> _themeCache` | 避免重复创建 |
| **引用跟踪** | `ResourceDictionary? _currentThemeDictionary` | 直接移除，无需查找 |
| **异步刷新** | `Dispatcher.BeginInvoke(DispatcherPriority.Loaded)` | 不阻塞 UI |
| **可见窗口刷新** | `Where(w => w.IsVisible)` | 只刷新可见窗口 |

#### 3.1.4 主题切换流程

```
用户请求切换主题
        ↓
ThemeManager.CurrentTheme = newTheme
        ↓
移除旧主题资源字典
        ↓
检查缓存，获取或创建新主题字典
        ↓
添加到 Application.Resources.MergedDictionaries
        ↓
异步刷新所有可见窗口
        ↓
触发 ThemeChanged 事件
```

### 3.2 控件注册系统 (ControlRegistry)

#### 3.2.1 设计目标
为工具箱、设计时支持、文档生成等场景提供统一的控件元数据。

#### 3.2.2 元数据结构

```csharp
public class ControlInfo
{
    public string Name { get; set; }                    // 控件类名
    public string DisplayName { get; set; }             // 显示名称（中文）
    public string Description { get; set; }             // 描述
    public ControlCategory Category { get; set; }       // 分类
    public ControlType Type { get; set; }               // 类型（Styled/Custom）
    public string Icon { get; set; }                    // 图标路径或几何数据
    public string XamlNamespace { get; set; }           // XAML 命名空间前缀
    public string XamlNamespaceUri { get; set; }        // XAML 命名空间 URI
    public string SampleCode { get; set; }              // 示例代码
    public List<ControlStyleVariant>? StyleVariants { get; set; } // 样式变体
}

public enum ControlType
{
    Styled,    // WPF 原生控件 + Win11 样式
    Custom     // Lemoo 自定义控件
}

public enum ControlCategory
{
    Buttons, Inputs, Lists, Menus, Progress, Sliders,
    Cards, Dialogs, Notifications, Navigation, Chrome, Others
}
```

#### 3.2.3 注册示例

```csharp
new ControlInfo
{
    Name = "Button",
    DisplayName = "按钮",
    Description = "基础按钮控件，支持多种样式变体",
    Category = ControlCategory.Buttons,
    Type = ControlType.Styled,
    SampleCode = "<Button Content=\"点击我\" />",
    StyleVariants = new List<ControlStyleVariant>
    {
        new() { StyleName = "Default", DisplayName = "默认" },
        new() { StyleName = "Primary", DisplayName = "主按钮", StyleKey = "Win11.Button.Primary" },
        new() { StyleName = "Outline", DisplayName = "轮廓按钮", StyleKey = "Win11.Button.Outline" },
        new() { StyleName = "Ghost", DisplayName = "幽灵按钮", StyleKey = "Win11.Button.Ghost" },
        new() { StyleName = "Danger", DisplayName = "危险按钮", StyleKey = "Win11.Button.Danger" }
    }
}
```

### 3.3 页面注册系统 (IPageRegistry)

#### 3.3.1 设计目标
支持模块化 UI 的页面注册和导航。

#### 3.3.2 接口定义

```csharp
public interface IPageRegistry
{
    void RegisterPage(string pageKey, Type pageType, NavigationItemMetadata metadata);
    Type? GetPageType(string pageKey);
    object? CreatePage(string pageKey, IServiceProvider serviceProvider);
    IEnumerable<NavigationItemMetadata> GetPagesByModule(string moduleName);
    IReadOnlyDictionary<string, NavigationItemMetadata> GetAllPages();
}
```

#### 3.3.3 导航元数据

```csharp
public class NavigationItemMetadata
{
    public string PageKey { get; set; }           // 页面键（唯一标识）
    public string Title { get; set; }             // 显示标题
    public string Icon { get; set; }              // 图标（Segoe MDL2 Assets）
    public string Module { get; set; }            // 所属模块名称
    public bool IsEnabled { get; set; }           // 是否启用
    public string? Description { get; set; }      // 描述
    public string? ParentPageKey { get; set; }    // 父导航项键
    public int Order { get; set; }                // 排序顺序
}
```

---

## 四、控件实现分析

### 4.1 控件实现模式

Lemoo.UI 的控件实现遵循统一的模式：

#### 4.1.1 基础结构

```csharp
[ToolboxItem(true)]
[Category(ToolboxCategories.Buttons)]
[Description("用于显示通知计数或状态指示的徽章控件")]
public class Badge : ContentControl
{
    #region Constructor

    static Badge()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Badge),
            new FrameworkPropertyMetadata(typeof(Badge)));
    }

    #endregion

    #region 依赖属性

    public static readonly DependencyProperty BadgeShapeProperty =
        DependencyProperty.Register(
            nameof(BadgeShape),
            typeof(BadgeShape),
            typeof(Badge),
            new PropertyMetadata(BadgeShape.Pill));

    public BadgeShape BadgeShape
    {
        get => (BadgeShape)GetValue(BadgeShapeProperty);
        set => SetValue(BadgeShapeProperty, value);
    }

    #endregion
}
```

#### 4.1.2 关键设计点

| 设计点 | 说明 |
|--------|------|
| **DefaultStyleKey** | 指定默认样式键，实现样式与控件的绑定 |
| **依赖属性** | 支持 WPF 数据绑定、样式、动画等特性 |
| **ToolboxItem** | 设计时工具箱支持 |
| **XML 注释** | 提供完整的文档和使用示例 |
| **命名约定** | 依赖属性使用 `Property` 后缀，包装器使用同名属性 |

### 4.2 典型控件分析

#### 4.2.1 Badge（徽章控件）

**职责**：显示通知计数或状态指示

**核心功能**：
- 多种形状：Pill, Circle, Rounded, Dot
- 多种位置：TopLeft, TopRight, BottomLeft, BottomRight, TopCenter, BottomCenter
- 最大值限制：超过显示 `99+`
- 零值显示控制

**实现要点**：
```csharp
public static readonly DependencyProperty MaxValueProperty =
    DependencyProperty.Register(
        nameof(MaxValue),
        typeof(int),
        typeof(Badge),
        new PropertyMetadata(99)); // 默认最大值 99
```

#### 4.2.2 AutoCompleteBox（自动完成文本框）

**职责**：带自动完成功能的文本输入

**核心功能**：
- 自动过滤建议列表
- 键盘导航（上下箭头、Enter、Esc）
- 最小前缀长度配置
- 双向绑定支持

**实现要点**：
```csharp
// 模板部件
private Popup? _popup;
private ListBox? _listBox;
private TextBox? _textBox;

public override void OnApplyTemplate()
{
    base.OnApplyTemplate();
    _textBox = GetTemplateChild("PART_TextBox") as TextBox;
    _listBox = GetTemplateChild("PART_ListBox") as ListBox;
    _popup = GetTemplateChild("PART_Popup") as Popup;
}

// 过滤逻辑
private void FilterItems()
{
    var filtered = ItemsSource.Cast<object>().Where(item =>
    {
        var displayText = GetDisplayText(item);
        return displayText != null &&
               displayText.IndexOf(Text, StringComparison.OrdinalIgnoreCase) >= 0;
    }).ToList();
    _listBox.ItemsSource = filtered;
    IsDropDownOpen = filtered.Count > 0;
}
```

#### 4.2.3 DialogHost（对话框宿主）

**职责**：在窗口内显示模态对话框

**核心功能**：
- 自定义对话框内容和模板
- 遮罩层配置（背景、透明度）
- 多种动画效果（FadeIn/Out, Slide, Zoom, ZoomFade）
- 点击外部关闭

**实现要点**：
```csharp
public enum DialogAnimation
{
    None,
    FadeIn,
    FadeOut,
    SlideFromTop,
    SlideFromBottom,
    SlideFromLeft,
    SlideFromRight,
    Zoom,
    ZoomFade
}

private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
{
    if (d is DialogHost dialogHost)
    {
        if ((bool)e.NewValue)
            dialogHost.OnOpened();
        else
            dialogHost.OnClosed();
    }
}
```

### 4.3 控件样式实现

#### 4.3.1 样式结构

每个控件的样式文件包含：

1. **资源字典引用**：引入依赖的令牌和动画
2. **共用动画**：定义可重用的 Storyboard
3. **控件样式**：定义控件的默认样式和变体
4. **控件模板**：定义控件的视觉树

#### 4.3.2 按钮样式示例

```xml
<ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source="pack://application:,,,/Lemoo.UI;component/Styles/Design/Animations.xaml"/>
    <ResourceDictionary Source="pack://application:,,,/Lemoo.UI;component/Styles/Win11/Win11.Tokens.xaml" />
</ResourceDictionary.MergedDictionaries>

<!-- 共用动画 Storyboard 资源 -->
<Storyboard x:Key="ButtonHoverEnterStoryboard">
    <ColorAnimation Storyboard.TargetName="border"
                   Storyboard.TargetProperty="(Border.Background).(SolidColorBrush.Color)"
                   To="{StaticResource button.hoverBackground}"
                   Duration="{StaticResource AnimationDuration.Normal}"
                   EasingFunction="{StaticResource EasingFunction.EaseOut}"/>
</Storyboard>

<!-- 默认样式 -->
<Style TargetType="{x:Type Button}" BasedOn="{StaticResource {x:Type Button}}">
    <Setter Property="Background" Value="{StaticResource Brush.Button.Background}" />
    <Setter Property="Foreground" Value="{StaticResource Brush.Button.Foreground}" />
    <Setter Property="BorderBrush" Value="{StaticResource Brush.Button.Border}" />
    <Setter Property="BorderThickness" Value="1" />
    <Setter Property="Padding" Value="{StaticResource Spacing.Padding.Medium}" />
    <Setter Property="CornerRadius" Value="{StaticResource Win11.CornerRadius.Small}" />
    <Setter Property="FontSize" Value="{StaticResource Typography.FontSize.Default}" />
    <Setter Property="FontWeight" Value="Normal" />
    <Setter Property="Cursor" Value="Hand" />
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="{x:Type Button}">
                <Grid>
                    <Border x:Name="border"
                            Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}"
                            CornerRadius="{TemplateBinding CornerRadius}"
                            SnapsToDevicePixels="True">
                        <ContentPresenter x:Name="contentPresenter"
                                          Focusable="False"
                                          HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}"
                                          Margin="{TemplateBinding Padding}"
                                          RecognizesAccessKey="True"
                                          SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}"
                                          VerticalAlignment="{TemplateBinding VerticalContentAlignment}"/>
                    </Border>
                </Grid>
                <ControlTemplate.Triggers>
                    <Trigger Property="IsMouseOver" Value="True">
                        <Trigger.EnterActions>
                            <BeginStoryboard Storyboard="{StaticResource ButtonHoverEnterStoryboard}"/>
                        </Trigger.EnterActions>
                        <Trigger.ExitActions>
                            <BeginStoryboard Storyboard="{StaticResource ButtonHoverExitStoryboard}"/>
                        </Trigger.ExitActions>
                    </Trigger>
                    <Trigger Property="IsPressed" Value="True">
                        <Trigger.EnterActions>
                            <BeginStoryboard Storyboard="{StaticResource ButtonPressedStoryboard}"/>
                        </Trigger.EnterActions>
                        <Trigger.ExitActions>
                            <BeginStoryboard Storyboard="{StaticResource ButtonReleasedStoryboard}"/>
                        </Trigger.ExitActions>
                    </Trigger>
                </ControlTemplate.Triggers>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>

<!-- Primary 变体 -->
<Style x:Key="Win11.Button.Primary" TargetType="{x:Type Button}" BasedOn="{StaticResource {x:Type Button}}">
    <Setter Property="Background" Value="{StaticResource Palette.Accent.Primary}" />
    <Setter Property="Foreground" Value="{StaticResource Palette.Text.Inverse}" />
    <Setter Property="BorderBrush" Value="Transparent" />
    <Setter Property="BorderThickness" Value="0" />
</Style>
```

---

## 五、样式和主题系统

### 5.1 设计系统层次

```
┌─────────────────────────────────────────────────────────────┐
│                  设计系统（Styles/Design）                   │
│  - Typography（字体）                                        │
│  - Spacing（间距）                                           │
│  - Shadows（阴影）                                           │
│  - Animations（动画）                                        │
├─────────────────────────────────────────────────────────────┤
│                  控件样式（Styles/Win11）                     │
│  - Win11.Tokens（控件令牌）                                  │
│  - Win11.Button（按钮样式）                                  │
│  - Win11.TextBox（文本框样式）                               │
│  - ...                                                       │
├─────────────────────────────────────────────────────────────┤
│                  主题系统（Themes）                           │
│  - ColorPalette（调色板）                                    │
│  - SemanticTokens（语义令牌）                                │
│  - ComponentBrushes（组件画刷）                              │
└─────────────────────────────────────────────────────────────┘
```

### 5.2 令牌系统

#### 5.2.1 设计令牌（Design Tokens）

| 文件 | 用途 |
|------|------|
| **Typography.xaml** | 字体、字号、行高 |
| **Spacing.xaml** | 间距、内边距、外边距 |
| **Shadows.xaml** | 阴影效果（卡片、浮动元素等） |
| **Animations.xaml** | 动画效果（过渡、缓动函数） |

#### 5.2.2 控件令牌（Win11 Tokens）

| 令牌类型 | 示例 |
|---------|------|
| **圆角** | `Win11.CornerRadius.Small`, `Win11.CornerRadius.Medium` |
| **字号** | `Win11.FontSize.Caption`, `Win11.FontSize.Body` |
| **间距** | `Win11.Spacing.XXSmall`, `Win11.Spacing.XSmall` |

#### 5.2.3 主题令牌（Theme Tokens）

| 层级 | 示例 |
|------|------|
| **调色板** | `Palette.Background.Primary`, `Palette.Accent.Primary` |
| **语义令牌** | `button.background`, `input.focusBorder`, `card.background` |
| **组件画刷** | `Brush.Button.Background`, `Brush.Input.Border` |

### 5.3 主题结构

#### 5.3.1 三层架构

```
ColorPalette.xaml（调色板 - 基础颜色）
        ↓
SemanticTokens.xaml（语义令牌 - 用途颜色）
        ↓
ComponentBrushes.xaml（组件画刷 - 实际 Brush）
```

#### 5.3.2 调色板示例（ColorPalette.xaml）

```xml
<!-- 主背景色系 -->
<Color x:Key="Palette.Background.Primary">#222831</Color>
<Color x:Key="Palette.Background.Secondary">#161A1E</Color>

<!-- 强调色系 -->
<Color x:Key="Palette.Accent.Primary">#00ADB5</Color>
<Color x:Key="Palette.Accent.Hover">#00C4CF</Color>

<!-- 文本色系 -->
<Color x:Key="Palette.Text.Primary">#EEEEEE</Color>
<Color x:Key="Palette.Text.Secondary">#DDDDDD</Color>
```

#### 5.3.3 语义令牌示例（SemanticTokens.xaml）

```xml
<!-- VS Code 风格语义令牌 -->
<Color x:Key="button.background">#00000000</Color>
<Color x:Key="button.foreground">#EEEEEE</Color>
<Color x:Key="button.hoverBackground">#333B46</Color>

<Color x:Key="input.background">#1A1E24</Color>
<Color x:Key="input.border">#2A3039</Color>
<Color x:Key="input.focusBorder">#00ADB5</Color>

<Color x:Key="card.background">#2E343D</Color>
<Color x:Key="card.border">#3A3F46</Color>
```

#### 5.3.4 组件画刷示例（ComponentBrushes.xaml）

```xml
<!-- 将颜色转换为 Brush，供控件使用 -->
<SolidColorBrush x:Key="Brush.Button.Background" Color="{StaticResource button.background}"/>
<SolidColorBrush x:Key="Brush.Button.Foreground" Color="{StaticResource button.foreground}"/>
<SolidColorBrush x:Key="Brush.Button.Border" Color="{StaticResource button.border}"/>
```

### 5.4 主题切换机制

#### 5.4.1 资源字典管理

```csharp
// 资源字典缓存
private static readonly Dictionary<Theme, ResourceDictionary> _themeCache = new();
private static ResourceDictionary? _currentThemeDictionary;

// 主题 URI（常量，避免重复创建）
private static readonly Uri BaseThemeUri = new("pack://application:,,,/Lemoo.UI;component/Themes/Base/Base.xaml");

// 应用主题
public static void ApplyTheme(Theme theme)
{
    // 1. 移除旧主题
    if (_currentThemeDictionary != null && app.Resources.MergedDictionaries.Contains(_currentThemeDictionary))
    {
        app.Resources.MergedDictionaries.Remove(_currentThemeDictionary);
    }

    // 2. 从缓存获取或创建新主题
    if (!_themeCache.TryGetValue(actualTheme, out var themeDict))
    {
        themeDict = new ResourceDictionary { Source = themeUri };
        _themeCache[actualTheme] = themeDict;
    }

    // 3. 添加新主题
    app.Resources.MergedDictionaries.Add(themeDict);
    _currentThemeDictionary = themeDict;

    // 4. 刷新窗口
    RefreshAllWindowsAsync();
}
```

#### 5.4.2 窗口刷新机制

```csharp
// 异步刷新，避免阻塞 UI
private static void RefreshAllWindowsAsync()
{
    app.Dispatcher.BeginInvoke(
        DispatcherPriority.Loaded,
        new Action(() => { RefreshVisibleWindows(); }));
}

// 只刷新可见窗口
private static void RefreshVisibleWindows()
{
    foreach (Window window in app.Windows.OfType<Window>().Where(w => w.IsVisible))
    {
        if (window is FrameworkElement fe)
        {
            RefreshElementResources(fe);
        }
    }
}
```

---

## 六、设计模式和最佳实践

### 6.1 设计模式应用

#### 6.1.1 注册表模式（Registry Pattern）

**应用场景**：控件注册、页面注册、主题管理

**优势**：
- 集中管理元数据
- 支持查询和搜索
- 易于扩展

**示例**：
```csharp
public static class ControlRegistry
{
    public static IReadOnlyList<ControlInfo> GetAllControls() => _controls;
    public static IReadOnlyList<ControlInfo> GetControlsByCategory(ControlCategory category)
        => _controls.Where(c => c.Category == category).ToList();
    public static IReadOnlyList<ControlInfo> SearchControls(string keyword)
        => _controls.Where(c => c.Name.Contains(keyword, ...)).ToList();
}
```

#### 6.1.2 模板方法模式（Template Method Pattern）

**应用场景**：控件基类、事件处理

**示例**：
```csharp
protected virtual void OnOpened()
{
    Focus();
    DialogOpened?.Invoke(this, new DialogOpenedEventArgs());
}

protected virtual void OnClosed()
{
    DialogClosed?.Invoke(this, new DialogClosedEventArgs());
}
```

#### 6.1.3 观察者模式（Observer Pattern）

**应用场景**：主题切换、属性变化通知

**示例**：
```csharp
public static event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

// 触发事件
ThemeChanged?.Invoke(null, new ThemeChangedEventArgs(oldTheme, value));
```

#### 6.1.4 策略模式（Strategy Pattern）

**应用场景**：动画类型、对话框样式

**示例**：
```csharp
public enum DialogAnimation
{
    None,
    FadeIn,
    FadeOut,
    SlideFromTop,
    SlideFromBottom,
    Zoom,
    ZoomFade
}
```

### 6.2 WPF 最佳实践

#### 6.2.1 依赖属性

```csharp
// ✅ 正确：使用 DependencyProperty
public static readonly DependencyProperty TextProperty =
    DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(MyControl),
        new FrameworkPropertyMetadata(string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnTextChanged));

public string Text
{
    get => (string)GetValue(TextProperty);
    set => SetValue(TextProperty, value);
}

// ❌ 错误：使用普通属性（不支持绑定、样式等）
public string Text { get; set; }
```

#### 6.2.2 路由事件

```csharp
// ✅ 正确：使用路由事件
public static readonly RoutedEvent TextChangedEvent =
    EventManager.RegisterRoutedEvent(
        nameof(TextChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(MyControl));

public event RoutedEventHandler TextChanged
{
    add => AddHandler(TextChangedEvent, value);
    remove => RemoveHandler(TextChangedEvent, value);
}

protected virtual void OnTextChanged()
{
    RaiseEvent(new RoutedEventArgs(TextChangedEvent, this));
}
```

#### 6.2.3 模板部件

```csharp
// ✅ 正确：使用 PART_ 前缀命名模板部件
public override void OnApplyTemplate()
{
    base.OnApplyTemplate();

    // 清理旧的事件订阅
    if (_textBox != null)
    {
        _textBox.TextChanged -= TextBox_TextChanged;
    }

    // 获取模板部件
    _textBox = GetTemplateChild("PART_TextBox") as TextBox;

    // 订阅新的事件
    if (_textBox != null)
    {
        _textBox.TextChanged += TextBox_TextChanged;
    }
}
```

#### 6.2.4 资源管理

```xml
<!-- ✅ 正确：使用 pack:// URI 引用资源 -->
<ResourceDictionary Source="pack://application:,,,/Lemoo.UI;component/Styles/Design/Typography.xaml" />

<!-- ❌ 错误：使用相对路径（可能在不同上下文中失败） -->
<ResourceDictionary Source="/Styles/Design/Typography.xaml" />
```

### 6.3 性能优化

#### 6.3.1 虚拟化

```xml
<!-- ✅ 对列表控件启用虚拟化 -->
<VirtualizingStackPanel.IsVirtualizing="True"
VirtualizingStackPanel.VirtualizationMode="Recycling" />
```

#### 6.3.2 冻结 Freezable

```csharp
// ✅ 冻结画刷，提升性能
var brush = new SolidColorBrush(Colors.Red);
brush.Freeze(); // 冻结后不可修改，但性能更好
```

#### 6.3.3 延迟加载

```xml
<!-- ✅ 使用 x:Load 延迟加载 -->
<Grid x:Load="False">
    <!-- 只有在需要时才会加载 -->
</Grid>
```

---

## 七、特色功能

### 7.1 工具箱集成

#### 7.1.1 设计时支持

```csharp
[ToolboxItem(true)]
[Category(ToolboxCategories.Buttons)]
[Description("用于显示通知计数或状态指示的徽章控件")]
public class Badge : ContentControl
{
}
```

#### 7.1.2 工具箱注册

```csharp
public static class ControlToolboxRegistrar
{
    public static void RegisterAll()
    {
        // 自动扫描并注册所有控件
        var controls = typeof(ControlToolboxRegistrar).Assembly
            .GetTypes()
            .Where(t => t.GetCustomAttributes<ToolboxItemAttribute>().Any())
            .Select(t => new
            {
                Type = t,
                Category = t.GetCustomAttribute<CategoryAttribute>()?.Category,
                Description = t.GetCustomAttribute<DescriptionAttribute>()?.Description
            });
    }
}
```

### 7.2 模块化 UI

#### 7.2.1 模块 UI 接口

```csharp
public interface IModuleUI
{
    string ModuleName { get; }
    void RegisterUI(IServiceProvider services);
    IEnumerable<NavigationItemMetadata> GetNavigationItems();
}
```

#### 7.2.2 页面注册

```csharp
public interface IPageRegistry
{
    void RegisterPage(string pageKey, Type pageType, NavigationItemMetadata metadata);
    Type? GetPageType(string pageKey);
    object? CreatePage(string pageKey, IServiceProvider serviceProvider);
}
```

### 7.3 转换器库

#### 7.3.1 常用转换器

| 转换器 | 功能 |
|--------|------|
| **BooleanToVisibilityConverter** | 布尔值转可见性 |
| **InverseBooleanToVisibilityConverter** | 反向布尔值转可见性 |
| **ArcSegmentConverter** | 创建圆弧段路径 |
| **EnumToStringConverter** | 枚举转字符串 |
| **CategoryExpandedConverter** | 分类展开状态转换 |

#### 7.3.2 转换器使用

```xml
<converters:BooleanToVisibilityConverter x:Key="BoolToVis" />

<TextBlock Visibility="{Binding IsVisible, Converter={StaticResource BoolToVis}}" />
```

---

## 八、项目统计

### 8.1 代码统计

| 分类 | 数量 |
|------|------|
| **自定义控件** | 15+ |
| **Win11 样式控件** | 15+ |
| **样式变体** | 50+ |
| **转换器** | 12+ |
| **主题** | 6 个 |
| **资源字典** | 40+ |

### 8.2 控件分类统计

| 分类 | 控件数 |
|------|--------|
| Buttons | 5 |
| Inputs | 8 |
| Lists | 2 |
| Menus | 3 |
| Progress | 2 |
| Sliders | 2 |
| Cards | 2 |
| Dialogs | 2 |
| Notifications | 2 |
| Navigation | 3 |
| Chrome | 1 |
| Others | 2 |

### 8.3 主题列表

| 主题 | 描述 | 风格 |
|------|------|------|
| **Base** | 原色模式（默认） | Lemoo 品牌色 |
| **Dark** | 深色模式 | VS Dark 风格 |
| **Light** | 浅色模式 | VS Light 风格 |
| **NeonCyberpunk** | 霓虹赛博朋克 | 未来科技感 |
| **Aurora** | 极光 | 北极光风格 |
| **SunsetTropics** | 热带夕阳 | 热带海滩风格 |

---

## 九、技术亮点

### 9.1 架构设计

1. **分层清晰**：控件层、样式层、主题层、服务层、抽象层
2. **职责单一**：每个组件职责明确，便于维护
3. **高内聚低耦合**：模块化设计，组件间依赖最小化
4. **可扩展性强**：支持自定义控件、主题、样式

### 9.2 性能优化

1. **资源字典缓存**：避免重复创建主题资源
2. **异步窗口刷新**：使用低优先级异步，不阻塞 UI
3. **可见窗口刷新**：只刷新可见窗口，减少开销
4. **引用跟踪**：直接引用当前主题字典，快速移除

### 9.3 开发体验

1. **完整的 XML 注释**：每个控件都有详细的文档
2. **示例代码**：提供使用示例和最佳实践
3. **设计时支持**：工具箱集成、设计时数据
4. **中文友好**：控件名称和描述支持中文

### 9.4 设计系统

1. **令牌化**：设计令牌、控件令牌、主题令牌
2. **语义化**：VS Code 风格的语义令牌
3. **三层主题架构**：调色板 → 语义令牌 → 组件画刷
4. **与主题解耦**：设计系统与主题系统分离

---

## 十、总结

Lemoo.UI 是一个**设计精良、功能完善的 WPF UI 组件库**，具有以下优势：

### 10.1 优势总结

| 方面 | 优势 |
|------|------|
| **功能完整性** | 涵盖所有常见 UI 场景的控件 |
| **现代化设计** | Windows 11 Fluent Design 风格 |
| **多主题支持** | 6 种内置主题，支持运行时切换 |
| **高性能** | 资源缓存、异步刷新、虚拟化等优化 |
| **开发体验** | 完整文档、示例代码、设计时支持 |
| **架构质量** | 清晰分层、模块化、可扩展 |
| **MVVM 友好** | 深度集成 CommunityToolkit.Mvvm |

### 10.2 适用场景

- **企业级 WPF 应用程序**：作为 UI 基础框架
- **快速原型开发**：丰富的控件和样式
- **学习 WPF 控件开发**：优秀的参考实现
- **主题系统研究**：完整的主题切换方案

### 10.3 未来方向

1. **更多控件**：图表、日历、数据网格等高级控件
2. **主题编辑器**：可视化主题编辑工具
3. **动画库**：预定义动画效果库
4. **无障碍支持**：增强可访问性
5. **性能监控**：性能分析和优化工具

---

## 附录

### A. 关键文件索引

| 文件路径 | 说明 |
|---------|------|
| `Helpers/ThemeManager.cs` | 主题管理器 |
| `Services/ControlRegistry.cs` | 控件注册表 |
| `Abstractions/IPageRegistry.cs` | 页面注册表接口 |
| `Styles/CommonStyles.xaml` | 样式系统入口 |
| `Themes/Base/Base.xaml` | Base 主题入口 |

### B. 依赖项

| 包名 | 版本 | 用途 |
|------|------|------|
| `Microsoft.Extensions.DependencyInjection` | 10.0.1 | 依赖注入 |
| `CommunityToolkit.Mvvm` | 8.4.0 | MVVM 工具包 |

### C. 参考资料

- [WPF 控件开发指南](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/controls/)
- [Windows 11 设计规范](https://www.microsoft.com/design/fluent/)
- [VS Code 主题文档](https://code.visualstudio.com/api/extension-capabilities/theming)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)

---

**文档版本**: 1.0.0
**生成日期**: 2026-01-15
**作者**: Claude Code Analysis
