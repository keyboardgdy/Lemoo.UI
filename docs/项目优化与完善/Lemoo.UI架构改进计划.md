# Lemoo.UI 架构改进计划

> 版本：1.0.0
> 更新日期：2026-01-14
> 状态：规划中

---

## 目录

- [一、项目背景与定位](#一项目背景与定位)
- [二、架构现状评估](#二架构现状评估)
- [三、核心问题分析](#三核心问题分析)
- [四、改进路线图](#四改进路线图)
- [五、控件库扩展规范](#五控件库扩展规范)
- [六、主题系统增强](#六主题系统增强)
- [七、性能优化方案](#七性能优化方案)
- [八、文档与生态建设](#八文档与生态建设)
- [九、参考框架对比](#九参考框架对比)
- [十、实施指南](#十实施指南)

---

## 一、项目背景与定位

### 1.1 项目愿景

Lemoo.UI 定位为 **企业级模块化 WPF 应用框架**，专注于为构建大型、可维护的 WPF 应用程序提供基础设施。

### 1.2 核心价值主张

| 价值维度      | 说明                  | 与竞品差异                |
| --------- | ------------------- | -------------------- |
| **模块化架构** | 内置模块化支持，支持插件式扩展     | Prism 复杂，Lemoo.UI 简单 |
| **主题系统**  | VS Code 风格语义化 Token | 独特设计                 |
| **企业集成**  | 原生支持 DDD、CQRS、事件驱动  | 无竞品提供                |
| **开发体验**  | 简洁 API，完整 MVVM 支持   | 类似 MaterialDesign    |

### 1.3 目标用户

- 企业内部 WPF 应用开发团队
- 需要构建长期维护的桌面应用的开发者
- 已采用 Lemoo 架构（DDD + Clean Architecture）的团队

---

## 二、架构现状评估

### 2.1 当前架构概览

```
Lemoo.UI/
├── Abstractions/          # 抽象层（6个接口/类）
├── Controls/              # 自定义控件（仅3个）
│   ├── Chrome/MainTitleBar.xaml
│   ├── Navigation/Sidebar.xaml
│   └── Tabs/DocumentTabHost.xaml
├── Converters/           # 值转换器（仅3个）
├── Helpers/              # 辅助类（2个）
├── Styles/               # 样式系统（13个XAML）
└── Themes/               # 主题系统（6个主题×4文件=24个XAML）
```

### 2.2 现有组件清单

#### 2.2.1 自定义控件（3个）

| 控件名             | 功能描述     | 完成度 | 问题      |
| --------------- | -------- | --- | ------- |
| MainTitleBar    | 自定义窗口标题栏 | 90% | 无       |
| Sidebar         | 侧边导航栏    | 85% | 搜索功能未实现 |
| DocumentTabHost | 文档标签页容器  | 80% | 无       |
|                 |          |     |         |

#### 2.2.2 样式覆盖（Win11风格）

| 控件类型 | 样式完成度 | 备注 |
|---------|-----------|------|
| Button | ✅ 100% | 含 Primary Button 变体 |
| TextBox | ✅ 100% | 含 PasswordBox |
| CheckBox | ✅ 100% | - |
| ComboBox | ✅ 100% | - |
| RadioButton | ✅ 100% | - |
| ListBox | ✅ 100% | - |
| ProgressBar | ✅ 100% | - |
| Slider | ✅ 100% | - |
| Separator | ✅ 100% | - |
| ToggleButton | ✅ 100% | - |
| ToolTip | ✅ 100% | - |
| ScrollBar | ✅ 100% | - |

#### 2.2.3 主题系统（6个主题）

```
✅ Base          - 基础主题定义
✅ Light         - 浅色主题
✅ Dark          - 深色主题
✅ NeonCyberpunk - 霓虹赛博朋克主题
✅ Aurora        - 极光主题
✅ SunsetTropics - 热带夕阳主题
```

### 2.3 架构评分

| 评估维度 | 得分 | 满分 | 说明 |
|---------|-----|------|------|
| **代码质量** | 8 | 10 | 代码规范，注释完善 |
| **架构设计** | 7 | 10 | 分层清晰，但扩展性不足 |
| **主题系统** | 9 | 10 | VS Code 风格，设计优秀 |
| **控件库** | 2 | 10 | 严重不足，仅3个控件 |
| **文档完善** | 3 | 10 | 仅有基础 README |
| **易用性** | 6 | 10 | API 简单，但功能有限 |
| **性能** | 7 | 10 | 有优化意识，不够全面 |
| **可扩展性** | 5 | 10 | 模块化设计，但缺少扩展点 |
| **测试覆盖** | 0 | 10 | 无任何测试 |

**综合评分：5.9/10**

---

## 三、核心问题分析

### 3.1 致命问题（P0 - 阻塞使用）

#### 问题 1：控件库严重不足

**现状**：
- 仅 3 个自定义控件
- 无法满足实际项目需求

**影响**：
- 开发者必须自行实现常用控件
- 无法作为独立 UI 框架使用
- 竞争力为零

**参考对比**：

| 框架 | 控件数量 | Lemoo.UI 差距 |
|-----|---------|--------------|
| MaterialDesignInXAML | 50+ | -47 |
| HandyControl | 80+ | -77 |
| Panuon.UI.Silver | 30+ | -27 |

#### 问题 2：缺少对话框系统

**现状**：
- 无 DialogHost 或类似实现
- 依赖原生 MessageBox（体验差）

**影响**：
- 无法实现模态对话框
- 无法实现非模态对话框
- 无法实现嵌入式对话框
- 用户体验差

#### 问题 3：缺少通知系统

**现状**：
- 无 Snackbar 或 Notification 实现

**影响**：
- 无法显示操作反馈
- 无法显示警告/错误提示
- 缺少用户交互机制

### 3.2 严重问题（P1 - 影响体验）

#### 问题 4：转换器库过少

**现状**：仅 3 个转换器

**缺少的常用转换器**（10+）：
- MathAddConverter、MathMultiplyConverter
- DateTimeFormatConverter
- StringFormatConverter
- NullToBoolConverter
- CountToVisibilityConverter
- EqualToConverter、GreaterThanConverter
- EnumToBooleanConverter
- ObjectTypeConverter

#### 问题 5：无 Behaviors 系统

**现状**：无任何 Behavior 实现

**缺少的关键 Behaviors**：
- EventToCommandBehavior（最常用）
- FocusBehavior
- NumericInputBehavior
- ValidationBehavior
- DragDropBehavior

### 3.3 中等问题（P2 - 影响扩展）

#### 问题 6：主题系统限制

**当前限制**：
1. 不支持主题自定义（不能修改颜色值）
2. 不支持主题继承
3. 不支持运行时颜色调整
4. 主题信息硬编码

#### 问题 7：缺少图标系统

**现状**：无图标库集成

**影响**：
- 开发者必须自行准备图标
- 缺少统一的图标风格
- 增加开发成本

#### 问题 8：项目配置问题

**问题**：
- 手动管理 Page 引用（维护成本高）
- 缺少 NuGet 包元数据
- 无源代码链接配置

### 3.4 轻微问题（P3 - 优化改进）

#### 问题 9：文档缺失

**缺失内容**：
- API 文档
- 使用示例
- 最佳实践指南
- 迁移指南

#### 问题 10：缺少测试

**现状**：无任何单元测试或集成测试

---

## 四、改进路线图

### 4.1 总体时间规划

```
Phase 1: 基础控件库 (1-2个月)
    ├─ P0: 对话框系统
    ├─ P0: 通知系统
    ├─ P0: 基础控件扩展
    └─ P0: 转换器和Behaviors

Phase 2: 主题增强 (2-3个月)
    ├─ P1: 主题自定义系统
    ├─ P1: 图标系统集成
    └─ P1: 动画库扩展

Phase 3: 生态建设 (3-4个月)
    ├─ P2: 文档和示例
    ├─ P2: NuGet 发布
    └─ P2: 社区建设

Phase 4: 高级特性 (4-6个月)
    ├─ P3: 插件系统
    ├─ P3: 设计器支持
    └─ P3: 国际化支持
```

### 4.2 Phase 1: 基础控件库（1-2个月）

#### 目标

实现 15+ 核心控件，满足 80% 的企业应用场景。

#### 控件清单

| 优先级 | 控件名 | 复杂度 | 工作量 | 说明 |
|-------|-------|-------|-------|------|
| P0 | Card | 低 | 1天 | 卡片容器 |
| P0 | DialogHost | 高 | 5天 | 对话框宿主 |
| P0 | Snackbar | 中 | 3天 | 通知条 |
| P0 | MessageBox | 中 | 2天 | 消息框替代品 |
| P0 | Badge | 低 | 1天 | 徽章 |
| P0 | ToggleSwitch | 中 | 2天 | 开关 |
| P0 | ProgressRing | 中 | 2天 | 环形进度 |
| P0 | DropDownButton | 中 | 2天 | 下拉按钮 |
| P0 | SplitView | 高 | 4天 | 分割视图 |
| P0 | Expander | 中 | 2天 | 扩展器 |
| P0 | NumericUpDown | 中 | 3天 | 数字输入 |
| P0 | DateTimePicker | 高 | 5天 | 日期时间选择器 |
| P0 | SearchBox | 低 | 2天 | 搜索框 |
| P0 | PasswordBox | 中 | 2天 | 增强密码框 |
| P0 | Separator | 低 | 0.5天 | 分隔符增强 |

**总工作量：约 38.5 天（约 2 个月）**

#### 转换器清单（新增 10+）

```csharp
// 数值运算转换器
public class MathAddConverter : IValueConverter
public class MathMultiplyConverter : IValueConverter
public class MathDivideConverter : IValueConverter

// 布尔转换器
public class BoolToObjectConverter : IValueConverter
public class InverseBoolConverter : IValueConverter
public class NullToBoolConverter : IValueConverter

// 可见性转换器
public class CountToVisibilityConverter : IValueConverter
public class NullToVisibilityConverter : IValueConverter
public class StringEmptyToVisibilityConverter : IValueConverter

// 比较转换器
public class EqualToConverter : IValueConverter
public class GreaterThanConverter : IValueConverter
public class LessThanConverter : IValueConverter

// 格式化转换器
public class StringFormatConverter : IValueConverter
public class DateTimeFormatConverter : IValueConverter
public class EnumToBooleanConverter : IValueConverter

// 类型转换器
public class ObjectTypeConverter : IValueConverter
```

#### Behaviors 清单（新增 5+）

```csharp
// 事件到命令
public class EventToCommandBehavior : Behavior<DependencyObject>
{
    public static readonly DependencyProperty EventProperty = ...
    public static readonly DependencyProperty CommandProperty = ...
    public static readonly DependencyProperty CommandParameterProperty = ...
}

// 焦点行为
public class FocusBehavior : Behavior<FrameworkElement>
{
    public static readonly DependencyProperty IsFocusedProperty = ...
    public static readonly DependencyProperty FocusOnLoadProperty = ...
}

// 数字输入行为
public class NumericInputBehavior : Behavior<TextBox>
{
    public static readonly DependencyProperty MinValueProperty = ...
    public static readonly DependencyProperty MaxValueProperty = ...
    public static readonly DependencyProperty DecimalPlacesProperty = ...
}

// 验证行为
public class ValidationBehavior : Behavior<TextBox>
{
    public static readonly DependencyProperty ValidationRuleProperty = ...
    public static readonly DependencyProperty ErrorTooltipProperty = ...
}

// 拖放行为
public class DragDropBehavior : Behavior<UIElement>
{
    public static readonly DependencyProperty DropCommandProperty = ...
    public static readonly DependencyProperty DragCommandProperty = ...
    public static readonly DependencyProperty DragDropTemplateProperty = ...
}
```

### 4.3 Phase 2: 主题系统增强（2-3个月）

#### 4.3.1 主题自定义系统

```csharp
/// <summary>
/// 主题自定义器接口
/// </summary>
public interface IThemeCustomizer
{
    /// <summary>
    /// 自定义颜色
    /// </summary>
    void CustomizeColors(Dictionary<string, Color> colors);

    /// <summary>
    /// 自定义画刷
    /// </summary>
    void CustomizeBrushes(Dictionary<string, Brush> brushes);

    /// <summary>
    /// 自定义字体
    /// </summary>
    void CustomizeTypography(Dictionary<string, double> fontSizes);

    /// <summary>
    /// 自定义圆角
    /// </summary>
    void CustomizeCornerRadius(Dictionary<string, CornerRadius> corners);
}

/// <summary>
/// 主题自定义器实现
/// </summary>
public class ThemeCustomizer : IThemeCustomizer
{
    public void CustomizeColors(Dictionary<string, Color> colors)
    {
        var resources = Application.Current.Resources;

        foreach (var kvp in colors)
        {
            var key = $"Color.{kvp.Key}";
            if (resources.Contains(key))
            {
                resources[key] = kvp.Value;
            }
        }

        ThemeManager.RefreshTheme();
    }

    public void CustomizeBrushes(Dictionary<string, Brush> brushes)
    {
        var resources = Application.Current.Resources;

        foreach (var kvp in brushes)
        {
            var key = $"Brush.{kvp.Key}";
            if (resources.Contains(key))
            {
                resources[key] = kvp.Value;
            }
        }
    }

    public void CustomizeTypography(Dictionary<string, double> fontSizes)
    {
        var resources = Application.Current.Resources;

        foreach (var kvp in fontSizes)
        {
            var key = $"FontSize.{kvp.Key}";
            if (resources.Contains(key))
            {
                resources[key] = kvp.Value;
            }
        }
    }

    public void CustomizeCornerRadius(Dictionary<string, CornerRadius> corners)
    {
        var resources = Application.Current.Resources;

        foreach (var kvp in corners)
        {
            var key = $"CornerRadius.{kvp.Key}";
            if (resources.Contains(key))
            {
                resources[key] = kvp.Value;
            }
        }
    }
}

/// <summary>
/// 主题自定义扩展方法
/// </summary>
public static class ThemeCustomizerExtensions
{
    public static IThemeCustomizer WithPrimaryColor(
        this IThemeCustomizer customizer,
        Color color)
    {
        customizer.CustomizeColors(new Dictionary<string, Color>
        {
            ["Primary"] = color,
            ["PrimaryHover"] = color.Lighten(0.1),
            ["PrimaryPressed"] = color.Darken(0.1)
        });
        return customizer;
    }

    public static IThemeCustomizer WithAccentColor(
        this IThemeCustomizer customizer,
        Color color)
    {
        customizer.CustomizeColors(new Dictionary<string, Color>
        {
            ["Accent"] = color,
            ["AccentHover"] = color.Lighten(0.1),
            ["AccentPressed"] = color.Darken(0.1)
        });
        return customizer;
    }

    public static IThemeCustomizer WithCornerRadius(
        this IThemeCustomizer customizer,
        string key,
        CornerRadius corner)
    {
        customizer.CustomizeCornerRadius(new Dictionary<string, CornerRadius>
        {
            [key] = corner
        });
        return customizer;
    }
}

/// <summary>
/// 使用示例
/// </summary>
public class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var customizer = new ThemeCustomizer();

        // 链式 API
        customizer
            .WithPrimaryColor(Colors.Blue)
            .WithAccentColor(Colors.Orange)
            .WithCornerRadius("Small", new CornerRadius(4))
            .WithCornerRadius("Medium", new CornerRadius(8));
    }
}
```

#### 4.3.2 主题继承系统

```csharp
/// <summary>
/// 主题基类
/// </summary>
public abstract class ThemeBase : ITheme
{
    public abstract string Name { get; }
    public abstract string DisplayName { get; }
    public abstract string Description { get; }
    public abstract string Category { get; }
    public abstract Color PreviewColor { get; }

    /// <summary>
    /// 获取主题资源字典
    /// </summary>
    public abstract ResourceDictionary GetResourceDictionary();

    /// <summary>
    /// 获取父主题
    /// </summary>
    protected virtual ITheme? ParentTheme => null;

    /// <summary>
    /// 合并父主题资源
    /// </summary>
    protected ResourceDictionary GetMergedResources()
    {
        var dict = new ResourceDictionary();

        // 先添加父主题资源
        if (ParentTheme != null)
        {
            var parentDict = ParentTheme.GetResourceDictionary();
            foreach (var key in parentDict.Keys)
            {
                dict[key] = parentDict[key];
            }
        }

        // 再添加当前主题资源（覆盖父主题）
        var currentDict = GetResourceDictionary();
        foreach (var key in currentDict.Keys)
        {
            dict[key] = currentDict[key];
        }

        return dict;
    }
}

/// <summary>
/// 继承主题示例
/// </summary>
public class CustomDarkTheme : ThemeBase
{
    public override string Name => "CustomDark";
    public override string DisplayName => "自定义深色主题";
    public override string Description => "基于深色主题的自定义主题";
    public override string Category => "Custom";
    public override Color PreviewColor => Color.FromArgb(255, 40, 40, 40);

    protected override ITheme? ParentTheme =>
        ThemeManager.GetTheme(Theme.Dark);

    public override ResourceDictionary GetResourceDictionary()
    {
        return new ResourceDictionary
        {
            Source = new Uri("/Lemoo.UI;component/Themes/Custom/CustomDark.xaml",
                UriKind.Relative)
        };
    }
}
```

#### 4.3.3 图标系统集成

```csharp
/// <summary>
/// 图标类型
/// </summary>
public enum IconKind
{
    // 导航图标
    Home,
    Settings,
    About,
    Help,

    // 操作图标
    Add,
    Edit,
    Delete,
    Save,
    Cancel,
    Refresh,

    // 状态图标
    Success,
    Warning,
    Error,
    Info,

    // ... 更多图标
}

/// <summary>
/// PackIcon 控件
/// </summary>
public class PackIcon : Control
{
    static PackIcon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PackIcon),
            new FrameworkPropertyMetadata(typeof(PackIcon)));
    }

    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(nameof(Kind), typeof(IconKind),
            typeof(PackIcon),
            new PropertyMetadata(IconKind.Home, OnKindChanged));

    public static readonly DependencyProperty SpinProperty =
        DependencyProperty.Register(nameof(Spin), typeof(bool),
            typeof(PackIcon),
            new PropertyMetadata(false, OnSpinChanged));

    public static readonly DependencyProperty SpinDurationProperty =
        DependencyProperty.Register(nameof(SpinDuration), typeof(double),
            typeof(PackIcon),
            new PropertyMetadata(1.0));

    public IconKind Kind
    {
        get => (IconKind)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public bool Spin
    {
        get => (bool)GetValue(SpinProperty);
        set => SetValue(SpinProperty, value);
    }

    public double SpinDuration
    {
        get => (double)GetValue(SpinDurationProperty);
        set => SetValue(SpinDurationProperty, value);
    }

    private static void OnKindChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var icon = (PackIcon)d;
        icon.UpdateData();
    }

    private static void OnSpinChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var icon = (PackIcon)d;
        icon.UpdateSpinState();
    }

    private void UpdateData()
    {
        var data = IconDataFactory.Create(Kind);
        SetValue(DataPropertyProperty, data);
    }

    private void UpdateSpinState()
    {
        VisualStateManager.GoToState(this,
            Spin ? "SpinState" : "NormalState", true);
    }

    internal static readonly DependencyProperty DataPropertyProperty =
        DependencyProperty.Register("DataProperty", typeof(string),
            typeof(PackIcon));
}

/// <summary>
/// 图标数据工厂
/// </summary>
public static class IconDataFactory
{
    private static readonly Dictionary<IconKind, string> IconData = new()
    {
        [IconKind.Home] = "M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8z",
        [IconKind.Settings] = "M19.14,12.94c0.04-0.3,0.06-0.61,0.06-0.94c0-0.32-0.02-0.64-0.07-0.94l2.03-1.58c0.18-0.14,0.23-0.41,0.12-0.61 l-1.92-3.32c-0.12-0.22-0.37-0.29-0.59-0.22l-2.39,0.96c-0.5-0.38-1.03-0.7-1.62-0.94L14.4,2.81c-0.04-0.24-0.24-0.41-0.48-0.41 h-3.84c-0.24,0-0.43,0.17-0.47,0.41L9.25,5.35C8.66,5.59,8.12,5.92,7.63,6.29L5.24,5.33c-0.22-0.08-0.47,0-0.59,0.22L2.74,8.87 C2.62,9.08,2.66,9.34,2.86,9.48l2.03,1.58C4.84,11.36,4.8,11.69,4.8,12s0.02,0.64,0.07,0.94l-2.03,1.58 c-0.18,0.14-0.23,0.41-0.12,0.61l1.92,3.32c0.12,0.22,0.37,0.29,0.59,0.22l2.39-0.96c0.5,0.38,1.03,0.7,1.62,0.94l0.36,2.54 c0.05,0.24,0.24,0.41,0.48,0.41h3.84c0.24,0,0.44-0.17,0.47-0.41l0.36-2.54c0.59-0.24,1.13-0.56,1.62-0.94l2.39,0.96 c0.22,0.08,0.47,0,0.59-0.22l1.92-3.32c0.12-0.22,0.07-0.47-0.12-0.61L19.14,12.94z M12,15.6c-1.98,0-3.6-1.62-3.6-3.6 s1.62-3.6,3.6-3.6s3.6,1.62,3.6,3.6S13.98,15.6,12,15.6z",
        // ... 更多图标路径
    };

    public static string Create(IconKind kind)
    {
        return IconData.TryGetValue(kind, out var data) ? data : string.Empty;
    }
}
```

### 4.4 Phase 3: 生态建设（3-4个月）

#### 4.4.1 文档结构

```
docs/
├── README.md                    # 项目主页
├── getting-started/
│   ├── installation.md          # 安装指南
│   ├── quick-start.md           # 快速开始
│   └── migration.md             # 迁移指南
├── concepts/
│   ├── architecture.md          # 架构设计
│   ├── theming.md               # 主题系统
│   └── modular-design.md        # 模块化设计
├── controls/
│   ├── overview.md              # 控件概览
│   ├── button.md                # 按钮控件
│   ├── dialog-host.md           # 对话框宿主
│   └── ...                      # 更多控件文档
├── api/
│   ├── Lemoo.UI.md              # API 参考
│   └── ...                      # 更多 API
├── samples/
│   ├── basic-usage.md           # 基础用法
│   ├── theming.md               # 主题示例
│   └── ...                      # 更多示例
└── appendix/
    ├── changelog.md             # 更新日志
    ├── contributing.md          # 贡献指南
    └── license.md               # 许可证
```

#### 4.4.2 NuGet 包配置

```xml
<!-- Lemoo.UI.csproj -->
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net6.0-windows7.0</TargetFramework>
    <UseWPF>true</UseWPF>

    <!-- NuGet 元数据 -->
    <PackageId>Lemoo.UI</PackageId>
    <Version>1.0.0</Version>
    <Authors>Your Name</Authors>
    <Company>Your Company</Company>
    <Product>Lemoo.UI</Product>
    <Description>Enterprise-grade modular WPF application framework with modern theming system</Description>
    <PackageTags>WPF;UI;Framework;Modular;DDD;CleanArchitecture</PackageTags>
    <PackageIcon>icon.png</PackageIcon>
    <PackageReadmeFile>README.md</PackageReadmeFile>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <PackageProjectUrl>https://github.com/yourusername/Lemoo.UI</PackageProjectUrl>
    <RepositoryUrl>https://github.com/yourusername/Lemoo.UI</RepositoryUrl>
    <RepositoryType>git</RepositoryType>

    <!-- 源代码链接 -->
    <PublishRepositoryUrl>true</PublishRepositoryUrl>
    <EmbedUntrackedSources>true</EmbedUntrackedSources>
    <IncludeSymbols>true</IncludeSymbols>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>

    <!-- 自动包含 Page 和 Resource -->
    <EnableDefaultPageItems>true</EnableDefaultPageItems>
    <EnableDefaultResourceItems>true</EnableDefaultResourceItems>
  </PropertyGroup>

  <ItemGroup>
    <None Include="..\..\images\icon.png" Pack="true" PackagePath="\" />
    <None Include="..\..\README.md" Pack="true" PackagePath="\" />
  </ItemGroup>

</Project>
```

### 4.5 Phase 4: 高级特性（4-6个月）

#### 4.5.1 插件系统

```csharp
/// <summary>
/// UI 插件接口
/// </summary>
public interface IUIPlugin
{
    /// <summary>
    /// 插件名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 插件版本
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// 插件描述
    /// </summary>
    string Description { get; }

    /// <summary>
    /// 初始化插件
    /// </summary>
    void Initialize(IServiceProvider services);

    /// <summary>
    /// 注册控件
    /// </summary>
    void RegisterControls(IControlRegistry registry);

    /// <summary>
    /// 注册主题
    /// </summary>
    void RegisterThemes(IThemeRegistry registry);

    /// <summary>
    /// 卸载插件
    /// </summary>
    void Shutdown();
}

/// <summary>
/// 控件注册表接口
/// </summary>
public interface IControlRegistry
{
    void Register<TControl>() where TControl : Control;
    void Register<TControl>(string key) where TControl : Control;
}

/// <summary>
/// 主题注册表接口
/// </summary>
public interface IThemeRegistry
{
    void Register(ITheme theme);
    void Unregister(string themeName);
}

/// <summary>
/// 插件管理器
/// </summary>
public interface IUIPluginManager
{
    /// <summary>
    /// 加载插件
    /// </summary>
    Task<PluginLoadResult> LoadAsync(string pluginPath);

    /// <summary>
    /// 卸载插件
    /// </summary>
    Task UnloadAsync(string pluginName);

    /// <summary>
    /// 获取所有已加载的插件
    /// </summary>
    IReadOnlyList<IUIPlugin> GetLoadedPlugins();

    /// <summary>
    /// 插件加载事件
    /// </summary>
    event EventHandler<PluginLoadedEventArgs>? PluginLoaded;

    /// <summary>
    /// 插件卸载事件
    /// </summary>
    event EventHandler<PluginUnloadedEventArgs>? PluginUnloaded;
}
```

#### 4.5.2 国际化支持

```csharp
/// <summary>
/// 本地化资源管理器
/// </summary>
public interface ILocalizationManager
{
    /// <summary>
    /// 当前文化
    /// </summary>
    CultureInfo CurrentCulture { get; set; }

    /// <summary>
    /// 获取本地化字符串
    /// </summary>
    string GetString(string key);

    /// <summary>
    /// 获取本地化字符串（带格式）
    /// </summary>
    string GetString(string key, params object[] args);

    /// <summary>
    /// 文化改变事件
    /// </summary>
    event EventHandler<CultureChangedEventArgs>? CultureChanged;
}

/// <summary>
/// 本地化扩展
/// </summary>
public class LocalizeExtension : MarkupExtension
{
    public LocalizeExtension(string key)
    {
        Key = key;
    }

    public string Key { get; }
    public string? Prefix { get; set; }
    public object? DefaultValue { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var manager = ServiceLocator.GetService<ILocalizationManager>();

        var fullKey = string.IsNullOrEmpty(Prefix)
            ? Key
            : $"{Prefix}.{Key}";

        var value = manager.GetString(fullKey);

        return value ?? DefaultValue ?? $"#{fullKey}#";
    }
}

<!-- 使用示例 -->
<TextBlock Text="{ui:Localize WelcomeMessage, Prefix=Home}" />
```

---

## 五、控件库扩展规范

### 5.1 控件命名规范

#### 5.1.1 命名模式

```
[功能描述] + [控件类型]

示例：
- Card（卡片）
- DialogHost（对话框宿主）
- ToggleSwitch（开关）
- ProgressRing（环形进度）
- NumericUpDown（数字上下控件）
```

#### 5.1.2 特殊后缀

| 后缀 | 含义 | 示例 |
|-----|------|------|
| Host | 容器类型 | DialogHost |
| Box | 输入框 | SearchBox |
| Picker | 选择器 | DatePicker |
| Viewer | 查看器 | PdfViewer |
| Editor | 编辑器 | TextEditor |
| Bar | 工具条 | MenuBar |
| Panel | 面板 | InfoPanel |

### 5.2 控件实现规范

#### 5.2.1 基础结构

```csharp
/// <summary>
/// [控件描述]
/// </summary>
/// <remarks>
/// [使用说明]
/// </remarks>
/// <example>
/// <code>
/// <!-- [使用示例] -->
/// <ui:Card Header="标题" Footer="页脚">
///     <TextBlock Text="内容" />
/// </ui:Card>
/// </code>
/// </example>
public class Card : ContentControl
{
    #region Constructor

    static Card()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Card),
            new FrameworkPropertyMetadata(typeof(Card)));
    }

    public Card()
    {
        // 默认样式
    }

    #endregion

    #region Dependency Properties

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(
            nameof(Header),
            typeof(object),
            typeof(Card),
            new PropertyMetadata(null));

    public static readonly DependencyProperty FooterProperty =
        DependencyProperty.Register(
            nameof(Footer),
            typeof(object),
            typeof(Card),
            new PropertyMetadata(null));

    // ... 更多属性

    #endregion

    #region Properties

    /// <summary>
    /// 获取或设置卡片头部内容
    /// </summary>
    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// 获取或设置卡片尾部内容
    /// </summary>
    public object Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    // ... 更多属性

    #endregion

    #region Methods

    // ... 方法

    #endregion

    #region Commands

    // ... 命令

    #endregion
}
```

#### 5.2.2 依赖属性规范

```csharp
// ✅ 正确示例
public static readonly DependencyProperty HeaderProperty =
    DependencyProperty.Register(
        nameof(Header),                    // 使用 nameof
        typeof(object),                    // 属性类型
        typeof(Card),                      // 所属类型
        new PropertyMetadata(null, OnHeaderChanged));  // 回调可选

public object Header
{
    get => GetValue(HeaderProperty);
    set => SetValue(HeaderProperty, value);
}

private static void OnHeaderChanged(DependencyObject d,
    DependencyPropertyChangedEventArgs e)
{
    var card = (Card)d;
    // 处理属性变化
}

// ❌ 错误示例
public static readonly DependencyProperty HeaderProperty =
    DependencyProperty.Register("Header", typeof(object), typeof(Card));  // 字符串硬编码
```

#### 5.2.3 附加属性规范

```csharp
public class CardAssist
{
    public static readonly DependencyProperty IsOutlinedProperty =
        DependencyProperty.RegisterAttached(
            "IsOutlined",
            typeof(bool),
            typeof(CardAssist),
            new PropertyMetadata(false));

    public static bool GetIsOutlined(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsOutlinedProperty);
    }

    public static void SetIsOutlined(DependencyObject obj, bool value)
    {
        obj.SetValue(IsOutlinedProperty, value);
    }
}

<!-- 使用 -->
<ui:Card ui:CardAssist.IsOutlined="True" />
```

### 5.3 控件样式规范

#### 5.3.1 样式文件组织

```
Styles/Win11/
├── Controls/
│   ├── Card.xaml                    # 卡片样式
│   ├── DialogHost.xaml              # 对话框样式
│   └── ...
├── Templates/
│   ├── Card.Template.xaml           # 卡片模板
│   └── ...
└── Themes/
    ├── Card.Dark.xaml                # 深色主题
    └── Card.Light.xaml               # 浅色主题
```

#### 5.3.2 样式定义规范

```xml
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:ui="clr-namespace:Lemoo.UI.Controls">

    <!-- 1. 默认样式 -->
    <Style TargetType="{x:Type ui:Card}" x:Key="{x:Type ui:Card}">
        <Setter Property="Background" Value="{DynamicResource Brush.Card.Background}" />
        <Setter Property="BorderBrush" Value="{DynamicResource Brush.Card.Border}" />
        <Setter Property="BorderThickness" Value="1" />
        <Setter Property="CornerRadius" Value="{DynamicResource CornerRadius.Card}" />
        <Setter Property="Padding" Value="{DynamicResource Spacing.Medium}" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type ui:Card}">
                    <Border Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}"
                            CornerRadius="{TemplateBinding CornerRadius}"
                            Padding="{TemplateBinding Padding}">
                        <Grid>
                            <!-- 头部 -->
                            <ContentPresenter Content="{TemplateBinding Header}"
                                            x:Name="PART_Header"
                                            Visibility="Collapsed" />

                            <!-- 内容 -->
                            <ContentPresenter Content="{TemplateBinding Content}"
                                            x:Name="PART_Content" />

                            <!-- 尾部 -->
                            <ContentPresenter Content="{TemplateBinding Footer}"
                                            x:Name="PART_Footer"
                                            Visibility="Collapsed" />
                        </Grid>
                        <ControlTemplate.Triggers>
                            <!-- 显示头部 -->
                            <DataTrigger Binding="{Binding Header, RelativeSource={RelativeSource Self}}"
                                        Value="{x:Null}">
                                <Setter TargetName="PART_Header" Property="Visibility" Value="Collapsed" />
                            </DataTrigger>
                            <DataTrigger Binding="{Binding Header, RelativeSource={RelativeSource Self}}"
                                        Value="{x:Null}">
                                <Setter TargetName="PART_Header" Property="Visibility" Value="Visible" />
                            </DataTrigger>

                            <!-- 显示尾部 -->
                            <DataTrigger Binding="{Binding Footer, RelativeSource={RelativeSource Self}}"
                                        Value="{x:Null}">
                                <Setter TargetName="PART_Footer" Property="Visibility" Value="Collapsed" />
                            </DataTrigger>
                        </ControlTemplate.Triggers>
                    </Border>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <!-- 2. 变体样式 -->
    <Style TargetType="{x:Type ui:Card}" x:Key="Card.Outlined" BasedOn="{StaticResource {x:Type ui:Card}}">
        <Setter Property="BorderThickness" Value="2" />
    </Style>

    <Style TargetType="{x:Type ui:Card}" x:Key="Card.Elevated" BasedOn="{StaticResource {x:Type ui:Card}}">
        <Setter Property="Effect">
            <Setter.Value>
                <DropShadowEffect Color="{DynamicResource Color.Shadow}"
                                  BlurRadius="8"
                                  Opacity="0.15"
                                  Direction="270" />
            </Setter.Value>
        </Setter>
    </Style>

</ResourceDictionary>
```

### 5.4 控件测试规范

```csharp
[TestClass]
public class CardTests
{
    [TestMethod]
    public void Constructor_Default_InitializesCorrectly()
    {
        // Arrange & Act
        var card = new Card();

        // Assert
        Assert.IsNotNull(card);
    }

    [TestMethod]
    public void Header_WhenSet_RaisesPropertyChanged()
    {
        // Arrange
        var card = new Card();
        object? headerValue = null;
        card.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Card.Header))
                headerValue = card.Header;
        };

        // Act
        card.Header = "Test Header";

        // Assert
        Assert.AreEqual("Test Header", headerValue);
    }

    [TestMethod]
    public void Content_WhenSet_UpdatesVisual()
    {
        // Arrange
        var card = new Card();
        var textBlock = new TextBlock { Text = "Test Content" };

        // Act
        card.Content = textBlock;
        card.ApplyTemplate();
        var contentPresenter = card.Template.FindName("PART_Content", card) as ContentPresenter;

        // Assert
        Assert.IsNotNull(contentPresenter);
        Assert.AreEqual(textBlock, contentPresenter.Content);
    }
}
```

### 5.5 控件文档规范

每个控件必须包含：

1. **XML 注释** - 公共 API 必须有完整的 XML 注释
2. **使用示例** - 至少 3 个使用示例
3. **XAML 参考** - 属性、事件、附加属性列表
4. **设计规范** - 尺寸、间距、颜色规范
5. **可访问性** - 键盘导航、屏幕阅读器支持

```xml
<!--
    ╔══════════════════════════════════════════════════════════════════════╗
    ║                           Card 控件                                  ║
    ╠══════════════════════════════════════════════════════════════════════╣
    ║                                                                      ║
    ║  用途：卡片容器，用于组织相关内容                                     ║
    ║                                                                      ║
    ║  示例：                                                               ║
    ║    <ui:Card Header="标题">                                           ║
    ║        <TextBlock Text="内容" />                                     ║
    ║    </ui:Card>                                                        ║
    ║                                                                      ║
    ║  属性：                                                               ║
    ║    - Header: object              卡片头部内容                         ║
    ║    - Content: object             卡片内容（继承自 ContentControl）    ║
    ║    - Footer: object              卡片尾部内容                         ║
    ║    - CornerRadius: CornerRadius  圆角半径                            ║
    ║                                                                      ║
    ║  变体样式：                                                           ║
    ║    - Card.Outlined                轮廓样式                            ║
    ║    - Card.Elevated                抬升样式（带阴影）                  ║
    ║                                                                      ║
    ║  设计规范：                                                           ║
    ║    - 默认圆角：8px                                                  ║
    ║    - 默认间距：16px                                                 ║
    ║    - 边框粗细：1px                                                  ║
    ║                                                                      ║
    ╚══════════════════════════════════════════════════════════════════════╝
-->
```

---

## 六、主题系统增强

### 6.1 主题资源结构

```
Themes/
├── Base/
│   ├── Colors/
│   │   ├── Primary.xaml          # 主色调
│   │   ├── Accent.xaml           # 强调色
│   │   ├── Neutral.xaml          # 中性色
│   │   └── Semantic.xaml         # 语义色
│   ├── Typography/
│   │   ├── FontFamilies.xaml     # 字体族
│   │   └── FontSizes.xaml        # 字体大小
│   ├── Spacing/
│   │   └── Spacing.xaml          # 间距系统
│   ├── Shapes/
│   │   ├── CornerRadius.xaml     # 圆角
│   │   └── BorderThickness.xaml  # 边框粗细
│   └── Effects/
│       ├── Shadows.xaml          # 阴影效果
│       └── Animations.xaml       # 动画定义
├── Light/
│   ├── ColorPalette.xaml         # 颜色定义
│   ├── SemanticTokens.xaml       # 语义化 Token
│   └── ComponentBrushes.xaml     # 组件画刷
├── Dark/
│   ├── ColorPalette.xaml
│   ├── SemanticTokens.xaml
│   └── ComponentBrushes.xaml
└── Custom/
    └── ...
```

### 6.2 颜色系统规范

#### 6.2.1 颜色命名规则

```
[类型].[名称].[状态]

示例：
- Color.Primary                    # 主色
- Color.Primary.Hover              # 主色悬停
- Color.Primary.Pressed            # 主色按下
- Color.Background.Default         # 背景色
- Color.Text.Primary               # 主要文本
- Color.Border.default             # 边框颜色
- Color.Semantic.Success           # 成功色
```

#### 6.2.2 颜色定义示例

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:sys="clr-namespace:System;assembly=mscorlib">

    <!-- 主色系 -->
    <Color x:Key="Color.Primary">#0078D4</Color>
    <Color x:Key="Color.Primary.Hover">#106EBE</Color>
    <Color x:Key="Color.Primary.Pressed">#005A9E</Color>
    <Color x:Key="Color.Primary.Disabled">#CCE3F5</Color>

    <!-- 强调色系 -->
    <Color x:Key="Color.Accent">#8E8CD8</Color>
    <Color x:Key="Color.Accent.Hover">#7B70CC</Color>
    <Color x:Key="Color.Accent.Pressed">#6855BA</Color>

    <!-- 中性色系 -->
    <Color x:Key="Color.Gray">#8A8886</Color>
    <Color x:Key="Color.Gray.Shade1">#605E5C</Color>
    <Color x:Key="Color.Gray.Shade2">#3B3A39</Color>
    <Color x:Key="Color.Gray.Tint1">#A19F9D</Color>
    <Color x:Key="Color.Gray.Tint2">#C8C6C4</Color>
    <Color x:Key="Color.Gray.Tint3">#E1DFDD</Color>

    <!-- 背景色系 -->
    <Color x:Key="Color.Background.Default">#FFFFFF</Color>
    <Color x:Key="Color.Background.Alternate">#F3F2F1</Color>
    <Color x:Key="Color.Background.Disabled">#F3F2F1</Color>

    <!-- 文本色系 -->
    <Color x:Key="Color.Text.Primary">#323130</Color>
    <Color x:Key="Color.Text.Secondary">#605E5C</Color>
    <Color x:Key="Color.Text.Disabled">#A19F9D</Color>

    <!-- 边框色系 -->
    <Color x:Key="Color.Border.Default">#8A8886</Color>
    <Color x:Key="Color.Border.Focused">#0078D4</Color>

    <!-- 语义色系 -->
    <Color x:Key="Color.Semantic.Success">#107C10</Color>
    <Color x:Key="Color.Semantic.Warning">#797775</Color>
    <Color x:Key="Color.Semantic.Error">#A80000</Color>
    <Color x:Key="Color.Semantic.Info">#0078D4</Color>

</ResourceDictionary>
```

### 6.3 语义化 Token 系统

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!-- VS Code 风格语义化 Token -->
    <SolidColorBrush x:Key="Brush.Text" Color="{DynamicResource Color.Text.Primary}" />
    <SolidColorBrush x:Key="Brush.Text.Secondary" Color="{DynamicResource Color.Text.Secondary}" />
    <SolidColorBrush x:Key="Brush.Text.Disabled" Color="{DynamicResource Color.Text.Disabled}" />

    <SolidColorBrush x:Key="Brush.Background" Color="{DynamicResource Color.Background.Default}" />
    <SolidColorBrush x:Key="Brush.Background.Secondary" Color="{DynamicResource Color.Background.Alternate}" />

    <SolidColorBrush x:Key="Brush.Border" Color="{DynamicResource Color.Border.Default}" />
    <SolidColorBrush x:Key="Brush.Border.Focus" Color="{DynamicResource Color.Border.Focused}" />

    <SolidColorBrush x:Key="Brush.Primary" Color="{DynamicResource Color.Primary}" />
    <SolidColorBrush x:Key="Brush.Primary.Hover" Color="{DynamicResource Color.Primary.Hover}" />
    <SolidColorBrush x:Key="Brush.Primary.Pressed" Color="{DynamicResource Color.Primary.Pressed}" />

    <SolidColorBrush x:Key="Brush.Semantic.Success" Color="{DynamicResource Color.Semantic.Success}" />
    <SolidColorBrush x:Key="Brush.Semantic.Warning" Color="{DynamicResource Color.Semantic.Warning}" />
    <SolidColorBrush x:Key="Brush.Semantic.Error" Color="{DynamicResource Color.Semantic.Error}" />
    <SolidColorBrush x:Key="Brush.Semantic.Info" Color="{DynamicResource Color.Semantic.Info}" />

</ResourceDictionary>
```

---

## 七、性能优化方案

### 7.1 虚拟化支持

```csharp
/// <summary>
/// 虚拟化集合
/// </summary>
public class VirtualizingObservableCollection<T> : ObservableCollection<T>,
    INotifyCollectionChanged
{
    private readonly int _pageSize;
    private readonly Func<int, int, IEnumerable<T>> _loadPage;

    private int _loadedPages;

    public VirtualizingObservableCollection(
        int pageSize,
        Func<int, int, IEnumerable<T>> loadPage)
    {
        _pageSize = pageSize;
        _loadPage = loadPage;
    }

    public void LoadMore(int itemCount)
    {
        var page = _loadedPages;
        var items = _loadPage(page, _pageSize);

        foreach (var item in items)
        {
            Add(item);
        }

        _loadedPages++;
    }
}
```

### 7.2 延迟加载

```xml
<!-- 使用 VirtualizingStackPanel -->
<ListBox ItemsSource="{Binding Items}">
    <ListBox.ItemsPanel>
        <ItemsPanelTemplate>
            <VirtualizingStackPanel />
        </ItemsPanelTemplate>
    </ListBox.ItemsPanel>
</ListBox>

<!-- 启用容器回收 -->
<ListBox VirtualizingPanel.IsVirtualizing="True"
         VirtualizingPanel.VirtualizationMode="Recycling"
         VirtualizingPanel.ScrollUnit="Pixel" />
```

### 7.3 资源优化

```csharp
/// <summary>
/// 资源缓存优化
/// </summary>
public static class ResourceCache
{
    private static readonly Dictionary<string, WeakReference> _cache = new();

    public static T GetOrCreate<T>(string key, Func<T> factory) where T : class
    {
        if (_cache.TryGetValue(key, out var weakRef) &&
            weakRef.IsAlive &&
            weakRef.Target is T target)
        {
            return target;
        }

        var value = factory();
        _cache[key] = new WeakReference(value);
        return value;
    }
}
```

---

## 八、文档与生态建设

### 8.1 文档生成工具

推荐使用 **DocFX** 生成 API 文档：

```bash
# 安装 DocFX
dotnet tool install -g docfx

# 初始化文档项目
docfx init -q

# 生成文档
docfx build

# 预览文档
docfx serve _site
```

### 8.2 示例应用结构

```
samples/
├── Lemoo.UI.Samples.Basic/         # 基础示例
│   ├── Controls/                   # 控件示例
│   ├── Theming/                    # 主题示例
│   └── Patterns/                   # 模式示例
├── Lemoo.UI.Samples.Advanced/      # 高级示例
│   ├── CustomControls/             # 自定义控件
│   ├── CustomThemes/               # 自定义主题
│   └── Integration/                # 集成示例
└── Lemoo.UI.Samples.RealWorld/     # 真实场景
    ├── TaskManager/                # 任务管理器
    └── Dashboard/                  # 仪表板
```

### 8.3 社区建设

1. **GitHub 仓库**
   - 完善的 README
   - 贡献指南
   - 行为准则
   - Issue 模板

2. **NuGet 发布**
   - 稳定版发布周期
   - 预览版通道
   - 变更日志

3. **博客和教程**
   - 设计理念文章
   - 使用教程
   - 最佳实践

---

## 九、参考框架对比

### 9.1 MaterialDesignInXAML

**优点**：
- 控件丰富（50+）
- 文档完善
- 社区活跃
- Material Design 规范

**缺点**：
- 设计风格固定（Material Design）
- 定制难度较大
- 包体积大

**可借鉴**：
- 控件设计模式
- 对话框系统
- Snackbar 实现
- 文档结构

### 9.2 HandyControl

**优点**：
- 控件最多（80+）
- 中文文档完善
- 示例丰富
- 国人友好

**缺点**：
- 设计风格不够现代
- 代码质量一般
- 性能问题

**可借鉴**：
- 控件覆盖面
- 中文文档经验
- 示例应用设计

### 9.3 Panuon.UI.Silver

**优点**：
- 现代 UI 设计
- 动画效果出色
- 代码质量高
- 性能优秀

**缺点**：
- 控件数量少（30+）
- 英文文档为主
- 社区较小

**可借鉴**：
- UI 设计风格
- 动画实现
- 性能优化

---

## 十、实施指南

### 10.1 开发环境配置

```bash
# 1. 克隆仓库
git clone https://github.com/yourusername/Lemoo.UI.git
cd Lemoo.UI

# 2. 恢复 NuGet 包
dotnet restore

# 3. 构建项目
dotnet build --configuration Release

# 4. 运行测试
dotnet test

# 5. 运行示例
dotnet run --project samples/Lemoo.UI.Samples.Basic
```

### 10.2 控件开发工作流

```
1. 创建控件类
   ├─ 定义依赖属性
   ├─ 实现命令
   └─ 添加 XML 注释

2. 创建控件样式
   ├─ 定义默认样式
   ├─ 创建控件模板
   └─ 实现视觉状态

3. 编写单元测试
   ├─ 属性测试
   ├─ 命令测试
   └─ 交互测试

4. 创建使用示例
   ├─ XAML 示例
   ├─ 代码示例
   └─ 文档说明

5. 代码审查
   ├─ 代码规范检查
   ├─ 性能评估
   └─ 安全审查
```

### 10.3 发布流程

```bash
# 1. 更新版本号
# 编辑 Lemoo.UI.csproj，更新 Version

# 2. 生成 NuGet 包
dotnet pack --configuration Release

# 3. 测试 NuGet 包
dotnet new console -n TestApp
cd TestApp
dotnet add package ../Lemoo.UI/bin/Release/Lemoo.UI.1.0.0.nupkg

# 4. 发布到 NuGet.org
dotnet nuget push ../Lemoo.UI/bin/Release/Lemoo.UI.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json

# 5. 创建 GitHub Release
gh release create v1.0.0 --notes "Release notes"
```

### 10.4 版本规划

| 版本 | 时间 | 主要内容 |
|-----|------|---------|
| v1.0.0 | - | 基础主题系统、现有控件 |
| v1.1.0 | +2月 | 15+ 核心控件、转换器库、Behaviors |
| v1.2.0 | +3月 | 对话框系统、通知系统 |
| v1.3.0 | +4月 | 主题自定义、图标系统 |
| v2.0.0 | +6月 | 插件系统、国际化支持 |
| v2.1.0 | +8月 | 设计器支持、高级特性 |

---

## 附录

### A. 控件开发检查清单

- [ ] 控件类有完整的 XML 注释
- [ ] 所有公共属性有 XML 注释
- [ ] 所有公共方法有 XML 注释
- [ ] 所有事件有 XML 注释
- [ ] 依赖属性使用 `nameof` 而非字符串
- [ ] 属性变化回调正确处理
- [ ] 控件样式已定义
- [ ] 控件模板已定义
- [ ] 视觉状态已实现
- [ ] 单元测试覆盖率 > 80%
- [ ] 至少 3 个使用示例
- [ ] 文档已编写
- [ ] 性能测试已通过

### B. 主题开发检查清单

- [ ] 所有颜色有语义化命名
- [ ] 所有画刷有语义化命名
- [ ] 支持 Dark 主题
- [ ] 支持 Light 主题
- [ ] 支持高对比度模式
- [ ] 所有控件样式已适配
- [ ] 预览图已生成
- [ ] 主题元数据已填写

### C. 发布检查清单

- [ ] 版本号已更新
- [ ] 变更日志已更新
- [ ] API 文档已生成
- [ ] 示例应用已测试
- [ ] 单元测试全部通过
- [ ] 性能测试已通过
- [ ] NuGet 包已生成
- [ ] Release Notes 已编写
- [ ] GitHub Release 已创建

---

**文档结束**

如有疑问或建议，请提交 Issue 或 Pull Request。
