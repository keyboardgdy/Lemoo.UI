# Lemoo.UI - WPF UI 框架库

Lemoo.UI 是一个专业的 WPF UI 组件框架库，提供丰富的自定义控件、样式、主题和工具类。

## 项目结构

```
Lemoo.UI/
├── Controls/              # 自定义控件
│   ├── Buttons/          # 按钮控件
│   ├── Inputs/           # 输入控件
│   ├── Data/             # 数据展示控件
│   ├── Navigation/       # 导航控件
│   ├── Layout/           # 布局控件
│   └── Feedback/         # 反馈控件
├── Styles/                # 样式和模板
│   └── CommonStyles.xaml
├── Themes/                # 主题系统
│   ├── Generic.xaml      # 默认主题（必须）
│   ├── Light/            # 浅色主题
│   │   ├── Light.xaml
│   │   ├── Colors.xaml
│   │   ├── Brushes.xaml
│   │   └── Typography.xaml
│   └── Dark/             # 深色主题
│       ├── Dark.xaml
│       ├── Colors.xaml
│       ├── Brushes.xaml
│       └── Typography.xaml
├── Converters/            # 值转换器
│   └── BoolToVisibilityConverter.cs
├── Behaviors/             # 行为（Behaviors）
├── Extensions/            # 扩展方法
├── Helpers/               # 辅助类
│   └── ThemeManager.cs    # 主题管理器
├── Resources/             # 资源文件（图标、图片等）
└── Properties/            # 程序集属性
    └── AssemblyInfo.cs
```

## 使用方法

### 1. 在 App.xaml 中引用主题

```xml
<Application x:Class="YourApp.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- 引用浅色主题 -->
                <ResourceDictionary Source="pack://application:,,,/Lemoo.UI;component/Themes/Light/Light.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### 2. 切换主题

```csharp
using Lemoo.UI.Helpers;

// 切换到深色主题
ThemeManager.CurrentTheme = ThemeManager.Theme.Dark;

// 切换到浅色主题
ThemeManager.CurrentTheme = ThemeManager.Theme.Light;
```

### 3. 使用值转换器

```xml
<Window xmlns:ui="clr-namespace:Lemoo.UI.Converters;assembly=Lemoo.UI">
    <Window.Resources>
        <ui:BoolToVisibilityConverter x:Key="BoolToVisibilityConverter"/>
    </Window.Resources>
    
    <TextBlock Visibility="{Binding IsVisible, Converter={StaticResource BoolToVisibilityConverter}}"/>
</Window>
```

## 特性

- ✅ 完整的主题系统（浅色/深色）
- ✅ 可复用的自定义控件
- ✅ 丰富的样式和模板
- ✅ 值转换器库
- ✅ 主题管理器
- ✅ 扩展方法支持
- ✅ 行为（Behaviors）支持

## 开发计划

- [ ] 添加更多自定义控件
- [ ] 完善样式系统
- [ ] 添加动画效果
- [ ] 支持更多主题
- [ ] 添加图标库
- [ ] 完善文档和示例

## 版本

当前版本：1.0.0

