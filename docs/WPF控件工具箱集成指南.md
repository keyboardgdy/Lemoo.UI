
# Lemoo.UI 工具箱集成指南

## 概述

本文档说明如何在Visual Studio的工具箱中按分类展示Lemoo.UI的全部控件，包括自定义控件和复写样式的核心控件。

## 设计架构

### 1. 分类系统

工具箱使用以下12个分类：

| 分类 | 说明 | 包含控件 |
|------|------|----------|
| Lemoo.UI - 按钮 | 按钮类控件 | Button, DropDownButton, ToggleButton, ToggleSwitch, Badge |
| Lemoo.UI - 输入 | 输入类控件 | TextBox, PasswordBox, NumericUpDown, SearchBox, CheckBox, RadioButton, ComboBox |
| Lemoo.UI - 列表 | 列表类控件 | ListBox |
| Lemoo.UI - 菜单 | 菜单类控件 | MenuItem |
| Lemoo.UI - 进度 | 进度类控件 | ProgressBar, ProgressRing |
| Lemoo.UI - 滑块 | 滑块类控件 | Slider |
| Lemoo.UI - 卡片 | 卡片类控件 | Card, Expander |
| Lemoo.UI - 对话框 | 对话框类控件 | DialogHost, MessageBox |
| Lemoo.UI - 通知 | 通知类控件 | Snackbar, ToolTip |
| Lemoo.UI - 导航 | 导航类控件 | Sidebar, DocumentTabHost |
| Lemoo.UI - 窗口装饰 | 窗口装饰类控件 | MainTitleBar |
| Lemoo.UI - 其他 | 其他控件 | ScrollBar, Separator |

### 2. 控件类型区分

- **Custom**: Lemoo.UI自定义控件（如Badge, Card等）
- **Styled**: WPF核心控件 + Win11样式（如Button, TextBox等）

## 实现方案

### 方案一：Visual Studio原生工具箱集成

#### 1. 添加工具箱属性

在每个控件类上添加以下属性：

```csharp
using System.ComponentModel;
using Lemoo.UI.Design;

[ToolboxItem(true)]
[ToolboxBitmap(typeof(Badge), "Badge.ico")]
[Category(ToolboxCategories.Buttons)]
[Description("用于显示通知计数或状态指示的徽章控件")]
public class Badge : ContentControl
{
    // ...
}
```

#### 2. 创建控件图标

在项目中添加图标文件（.ico格式），推荐尺寸：
- 16x16（小图标）
- 32x32（标准图标）

#### 3. 配置项目文件

在 `.csproj` 文件中添加设计时支持：

```xml
<PropertyGroup>
  <!-- 启用设计时构建 -->
  <EnableDesignTimeBuild>true</EnableDesignTimeBuild>

  <!-- 设计时程序集位置 -->
  <DesignTimeBuildOutputPath>$(OutputPath)Design\</DesignTimeBuildOutputPath>
</PropertyGroup>

<ItemGroup>
  <!-- 嵌入图标资源 -->
  <EmbeddedResource Include="Design\Icons\*.ico" />

  <!-- 添加设计时引用 -->
  <DesignTimeReference Include="PresentationCore" />
  <DesignTimeReference Include="PresentationFramework" />
  <DesignTimeReference Include="WindowsBase" />
</ItemGroup>
```

#### 4. 注册控件程序集

在Visual Studio中，控件会自动出现在工具箱中，如果未出现：

1. 右键工具箱 → "选择项"
2. 浏览到 Lemoo.UI.dll
3. 勾选要显示的控件

### 方案二：应用内工具箱视图

使用已实现的 `ToolboxView` 用户控件：

```xaml
<Window x:Class="MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:ui="clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI"
        xmlns:vm="clr-namespace:Lemoo.UI.ViewModels;assembly=Lemoo.UI">

    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="300" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <!-- 工具箱 -->
        <ui:ToolboxView Grid.Column="0" />

        <!-- 设计区域 -->
        <Border Grid.Column="1" Background="White">
            <!-- 你的设计区域 -->
        </Border>
    </Grid>
</Window>
```

### 方案三：使用元数据注册系统

使用 `ToolboxInfoProvider` 获取控件信息：

```csharp
using Lemoo.UI.Design;

// 获取所有控件
var allControls = ToolboxInfoProvider.GetAllControls();

// 根据分类获取
var buttonControls = ToolboxInfoProvider.GetControlsByCategory(ToolboxCategories.Buttons);

// 搜索控件
var results = ToolboxInfoProvider.SearchControls("button");
```

## 控件注册规范

### 1. 新增自定义控件

```csharp
using System.ComponentModel;
using Lemoo.UI.Design;

namespace Lemoo.UI.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(MyCustomControl), "MyCustomControl.ico")]
    [Category(ToolboxCategories.Cards)]  // 选择合适的分类
    [Description("控件描述")]
    public class MyCustomControl : ContentControl
    {
        // 控件实现
    }
}
```

### 2. 新增核心控件样式

在 `CoreControlsToolboxRegistration` 中添加：

```csharp
public static class MyCoreControl
{
    public static string Description => "控件描述";
    public static string Category => ToolboxCategories.Inputs;
    public static string ExampleCode => "<MyControl Style=\"{StaticResource Win11.MyControlStyle}\" />";
}
```

然后在 `ToolboxInfoProvider.GetAllControls()` 中注册：

```csharp
controls.Add(new ControlToolboxInfo
{
    ControlName = "MyCoreControl",
    DisplayName = "我的控件",
    Description = CoreControlsToolboxRegistration.MyCoreControl.Description,
    Category = CoreControlsToolboxRegistration.MyCoreControl.Category,
    ExampleCode = CoreControlsToolboxRegistration.MyCoreControl.ExampleCode,
    IsCustomControl = false,
    XamlNamespacePrefix = ""
});
```

## 最佳实践

### 1. 图标设计

- 使用一致的视觉风格
- 提供清晰的控件类型标识
- 建议使用Fluent Design风格

### 2. 命名规范

- 控件名称使用PascalCase
- 显示名称使用中文
- 描述简洁明了，不超过20个字符

### 3. 分类原则

- 按功能分组，而非继承关系
- 每个分类最多包含10-15个控件
- 保持分类数量在10个以内

### 4. 示例代码

```csharp
// 自定义控件示例
"<ui:MyControl Property=\"Value\" />"

// 核心控件示例
"<MyControl Style=\"{StaticResource Win11.MyControlStyle}\" Property=\"Value\" />"
```

## 常见问题

### Q1: 控件没有出现在工具箱中？

**A:** 检查以下几点：
1. 控件类是否为 public
2. 是否添加了 [ToolboxItem(true)]
3. 控件是否有无参构造函数
4. 程序集是否已编译

### Q2: 分类显示不正确？

**A:** 确保：
1. Category 属性值正确
2. ToolboxCategories 中定义了该分类
3. 重新生成解决方案

### Q3: 图标没有显示？

**A:** 检查：
1. 图标文件是否嵌入为资源
2. ToolboxBitmap 属性路径是否正确
3. 图标尺寸是否合适（16x16或32x32）

## 相关文件

| 文件 | 说明 |
|------|------|
| `Design/ToolboxCategories.cs` | 分类常量定义 |
| `Design/ControlToolboxRegistrar.cs` | 控件注册系统 |
| `Controls/Toolbox/ToolboxView.xaml` | 工具箱视图 |
| `ViewModels/ToolboxViewModel.cs` | 工具箱视图模型 |
| `Models/ControlInfo.cs` | 控件信息模型 |

## 扩展阅读

- [WPF工具箱集成](https://docs.microsoft.com/en-us/dotnet/framework/wpf/controls/wpf-extensibility)
- [设计时特性](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.design.toolboxitemattribute)
- [Fluent Design设计规范](https://www.microsoft.com/design/fluent/)
