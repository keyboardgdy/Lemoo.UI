# Lemoo.UI WPF 控件库文档

## 概述

Lemoo.UI 是一套基于 WPF 的现代化 UI 控件库，遵循 Windows 11 设计语言，提供丰富的主题系统和完整的控件集。本文档详细列出了所有控件的完成状态、使用方式和后续规划。

## 文档信息

- **版本**: 1.1.0
- **最后更新**: 2026-01-15
- **设计规范**: Windows 11 Fluent Design
- **支持的 .NET 版本**: .NET 6.0+

---

## 目录

1. [控件完成度总览](#控件完成度总览)
2. [已完成控件详解](#已完成控件详解)
3. [控件分类索引](#控件分类索引)
4. [主题系统](#主题系统)
5. [待实现控件](#待实现控件)
6. [优化方向](#优化方向)
7. [开发路线图](#开发路线图)

---

## 控件完成度总览

| 控件名称            | 完成度  | 状态      | 文件位置                                  | 优先级 |
| --------------- | ---- | ------- | ------------------------------------- | --- |
| Button          | 100% | ✅ 完成    | Win11.Button.xaml                     | -   |
| TextBox         | 100% | ✅ 完成    | Win11.TextBox.xaml                    | -   |
| PasswordBox     | 100% | ✅ 完成    | Win11.PasswordBox.xaml                | -   |
| CheckBox        | 100% | ✅ 完成    | Win11.CheckBox.xaml                   | -   |
| RadioButton     | 100% | ✅ 完成    | Win11.Controls.xaml                   | -   |
| Badge           | 100% | ✅ 完成    | Controls/Buttons/Badge.cs             | -   |
| ToggleSwitch    | 100% | ✅ 完成    | Controls/Buttons/ToggleSwitch.cs      | -   |
| DropDownButton  | 100% | ✅ 完成    | Controls/Buttons/DropDownButton.cs    | -   |
| Card            | 100% | ✅ 完成    | Controls/Cards/Card.cs                | -   |
| Expander        | 100% | ✅ 完成    | Controls/Cards/Expander.cs            | -   |
| SearchBox       | 100% | ✅ 完成    | Controls/Inputs/SearchBox.cs          | -   |
| NumericUpDown   | 100% | ✅ 完成    | Controls/Inputs/NumericUpDown.cs      | -   |
| ProgressRing    | 100% | ✅ 完成    | Controls/Progress/ProgressRing.cs     | -   |
| DialogHost      | 100% | ✅ 完成    | Controls/Dialogs/DialogHost.cs        | -   |
| MessageBox      | 100% | ✅ 完成    | Controls/Dialogs/MessageBox.cs        | -   |
| Snackbar        | 100% | ✅ 完成    | Controls/Notifications/Snackbar.cs    | -   |
| ScrollBar       | 100% | ✅ 完成    | Win11.ScrollBar.xaml                  | -   |
| Sidebar         | 95%  | ✅ 完成    | Controls/Navigation/Sidebar.xaml.cs   | -   |
| MainTitleBar    | 90%  | ✅ 完成    | Controls/Chrome/MainTitleBar.xaml.cs  | -   |
| DocumentTabHost | 90%  | ✅ 完成    | Controls/Tabs/DocumentTabHost.xaml.cs | -   |
| ToolboxView     | 85%  | ✅ 完成    | Controls/Toolbox/ToolboxView.xaml.cs  | -   |
| ComboBox        | 90%  | 🟡 基础完成 | Win11.Controls.xaml                   | 中   |
| ComboBoxItem    | 90%  | 🟡 基础完成 | Win11.Controls.xaml                   | 中   |
| ListBox         | 90%  | 🟡 基础完成 | Win11.Controls.xaml                   | 中   |
| ListBoxItem     | 90%  | 🟡 基础完成 | Win11.Controls.xaml                   | 中   |
| MenuItem        | 90%  | 🟡 基础完成 | Win11.Controls.xaml                   | 中   |
| ProgressBar     | 100% | ✅ 完成    | Win11.Controls.xaml                   | -   |
| Slider          | 90%  | 🟡 基础完成 | Win11.Controls.xaml                   | 中   |
| Separator       | 100% | ✅ 完成    | Win11.Controls.xaml                   | -   |
| ToggleButton    | 90%  | 🟡 基础完成 | Win11.Controls.xaml                   | 中   |
| ToolTip         | 90%  | 🟡 基础完成 | Win11.Controls.xaml                   | 中   |
| DataGrid        | 0%   | 🔴 未实现  | -                                     | 高   |
| TreeView        | 0%   | 🔴 未实现  | -                                     | 高   |
| TabControl      | 0%   | 🔴 未实现  | -                                     | 高   |
| ListView        | 0%   | 🔴 未实现  | -                                     | 中   |
| DatePicker      | 0%   | 🔴 未实现  | -                                     | 中   |
| TimePicker      | 0%   | 🔴 未实现  | -                                     | 中   |
| Calendar        | 0%   | 🔴 未实现  | -                                     | 中   |
| SplitView       | 0%   | 🔴 未实现  | -                                     | 中   |
| AutoCompleteBox | 0%   | 🔴 未实现  | -                                     | 中   |
| RichTextBox     | 0%   | 🔴 未实现  | -                                     | 低   |
| MenuBar         | 30%  | 🟡 部分完成 | -                                     | 低   |
| StatusBar       | 0%   | 🔴 未实现  | -                                     | 低   |
| Toolbar         | 0%   | 🔴 未实现  | -                                     | 低   |
| Ribbon          | 0%   | 🔴 未实现  | -                                     | 低   |

---


## 导航工具栏目录结构
```
工具箱
├── 搜索框
├── 按钮
│   ├── Button
│   ├── DropDownButton
│   ├── ToggleButton
│   ├── ToggleSwitch
│   └── Badge
├── 输入
│   ├── TextBox
│   ├── PasswordBox
│   ├── NumericUpDown
│   ├── SearchBox
│   ├── CheckBox
│   ├── RadioButton
│   └── ComboBox
├── 列表
│   ├── ListBox
│   └── ListBoxItem
├── 菜单
│   └── MenuItem
├── 进度
│   ├── ProgressBar
│   └── ProgressRing
├── 滑块
│   └── Slider
├── 卡片
│   ├── Card
│   └── Expander
├── 对话框
│   ├── DialogHost
│   └── MessageBox
├── 通知
│   ├── Snackbar
│   └── ToolTip
├── 导航
│   ├── Sidebar（侧边栏导航）
│   └── DocumentTabHost（文档标签页）
├── 窗口装饰
│   └── MainTitleBar（自定义标题栏）
├── 开发工具
│   └── ToolboxView（控件工具箱）
└── 其他
    ├── ScrollBar
    └── Separator

```

## 已完成控件详解

### 1. 基础控件

#### Button（按钮）
- **文件位置**: `Styles/Win11/Win11.Button.xaml`
- **完成度**: 100%
- **特性**:
  - 支持多种视觉样式（Primary、Secondary、Ghost）
  - 流畅的悬停和点击动画
  - 圆角半径可配置
  - 完整的禁用状态
- **使用示例**:
```xml
<!-- 基础按钮 -->
<Button Style="{StaticResource Win11.ButtonStyle}" Content="默认按钮"/>

<!-- 主按钮 -->
<Button Style="{StaticResource Win11.Button.Primary}" Content="主要按钮"/>

<!-- 轮廓按钮 -->
<Button Style="{StaticResource Win11.Button.Outline}" Content="轮廓按钮"/>

<!-- 幽灵按钮 -->
<Button Style="{StaticResource Win11.Button.Ghost}" Content="幽灵按钮"/>

<!-- 危险按钮 -->
<Button Style="{StaticResource Win11.Button.Danger}" Content="删除"/>

```

#### TextBox（文本框）
- **文件位置**: `Styles/Win11/Win11.TextBox.xaml`
- **完成度**: 100%
- **特性**:
  - 聚焦时的边框高亮效果
  - 悬停状态提示
  - 错误状态显示
  - 清空按钮（可选）

#### PasswordBox（密码框）
- **文件位置**: `Styles/Win11/Win11.PasswordBox.xaml`
- **完成度**: 100%
- **特性**:
  - 密码显示/隐藏切换
  - 与 TextBox 一致的外观

#### CheckBox（复选框）
- **文件位置**: `Styles/Win11/Win11.CheckBox.xaml`
- **完成度**: 100%
- **特性**:
  - 三态支持（Checked、Unchecked、Indeterminate）
  - 流畅的切换动画
  - 自定义标签位置

#### RadioButton（单选按钮）
- **文件位置**: `Styles/Win11/Win11.Controls.xaml`
- **完成度**: 100%
- **特性**:
  - 圆形选择器设计
  - 选中动画效果
  - 支持分组

### 2. 按钮控件

#### Badge（徽章）
- **文件位置**: `Controls/Buttons/Badge.cs`
- **完成度**: 100%
- **特性**:
  - 多种形状（Pill、Circle、Rounded、Dot）
  - 6种位置选项（四角+上下居中）
  - 数字最大值限制（99+）
  - 零值显示控制
- **使用示例**:
```xml
<ui:Badge Content="5" />
<ui:Badge Content="99+" BadgeShape="Circle" />
<ui:Badge BadgeShape="Dot" />
<ui:Badge Content="New" BadgePlacement="TopRight" />
```

#### ToggleSwitch（切换开关）
- **文件位置**: `Controls/Buttons/ToggleSwitch.cs`
- **完成度**: 100%
- **特性**:
  - 流畅的滑动动画
  - 自定义开关标签
  - Header 标签支持
  - 与 CheckBox 类似的 API
- **使用示例**:
```xml
<ui:ToggleSwitch Header="WiFi" />
<ui:ToggleSwitch Header="飞行模式" OnLabel="开启" OffLabel="关闭" />
<ui:ToggleSwitch Header="蓝牙" IsChecked="True" IsEnabled="False" />
```

#### DropDownButton（下拉按钮）
- **文件位置**: `Controls/Buttons/DropDownButton.cs`
- **完成度**: 100%
- **特性**:
  - 自定义下拉内容
  - 灵活的定位选项
  - 打开/关闭事件
  - 支持图标和文本
- **使用示例**:
```xml
<ui:DropDownButton Content="选项">
    <ui:DropDownButton.DropDownContent>
        <Border Background="{DynamicResource WorkbenchCardBrush}">
            <StackPanel>
                <Button Content="新建" />
                <Button Content="打开" />
                <Separator />
                <Button Content="退出" />
            </StackPanel>
        </Border>
    </ui:DropDownButton.DropDownContent>
</ui:DropDownButton>
```

### 3. 卡片和容器控件

#### Card（卡片）
- **文件位置**: `Controls/Cards/Card.cs`
- **完成度**: 100%
- **特性**:
  - 可配置的阴影效果
  - 圆角半径设置
  - 悬停效果（可选）
  - 内边距和背景色自定义
- **使用示例**:
```xml
<ui:Card Padding="16" CornerRadius="8" elevation="2">
    <StackPanel>
        <TextBlock Text="标题" FontWeight="SemiBold" />
        <TextBlock Text="内容" Margin="0,8,0,0" />
    </StackPanel>
</ui:Card>
```

#### Expander（扩展器）
- **文件位置**: `Controls/Cards/Expander.cs`
- **完成度**: 100%
- **特性**:
  - 四个展开方向（下、上、左、右）
  - 展开/折叠事件
  - 自定义 Header 模板
  - 流畅的动画效果
- **使用示例**:
```xml
<ui:Expander Header="详细信息">
    <TextBlock Text="这是展开的内容" />
</ui:Expander>
<ui:Expander Header="向右展开" ExpandDirection="Right" IsExpanded="True" />
```

### 4. 输入控件

#### SearchBox（搜索框）
- **文件位置**: `Controls/Inputs/SearchBox.cs`
- **完成度**: 100%
- **特性**:
  - 内置搜索图标
  - 清空按钮
  - 实时搜索事件
  - 占位符文本支持
- **使用示例**:
```xml
<ui:SearchBox PlaceholderText="搜索..." />
<ui:SearchBox Query="{Binding SearchQuery, Mode=TwoWay}" />
```

#### NumericUpDown（数字输入框）
- **文件位置**: `Controls/Inputs/NumericUpDown.cs`
- **完成度**: 100%
- **特性**:
  - 增加/减少按钮
  - 最小值/最大值限制
  - 小数位数设置
  - 增量步长配置
  - 只读模式
  - 值变更事件
- **使用示例**:
```xml
<ui:NumericUpDown Value="{Binding Quantity}" />
<ui:NumericUpDown Value="{Binding Price}" Minimum="0" Maximum="100" Increment="0.01" DecimalPlaces="2" />
<ui:NumericUpDown Value="50" IsReadOnly="True" />
```

### 5. 进度控件

#### ProgressRing（环形进度条）
- **文件位置**: `Controls/Progress/ProgressRing.cs`
- **样式文件**: `Styles/Win11/Win11.ProgressRing.xaml`
- **完成度**: 100%
- **特性**:
  - 确定性和不确定性模式
  - 可配置环的粗细
  - 百分比文本显示（可选）
  - 自定义大小和颜色
- **使用示例**:
```xml
<ui:ProgressRing Value="50" Maximum="100" />
<ui:ProgressRing IsIndeterminate="True" />
<ui:ProgressRing Width="100" Height="100" Value="75" ShowPercentage="True" />
```

### 6. 对话框和通知

#### DialogHost（对话框宿主）
- **文件位置**: `Controls/Dialogs/DialogHost.cs`
- **样式文件**: `Styles/Win11/Win11.DialogHost.xaml`
- **完成度**: 100%
- **特性**:
  - 在窗口内显示模态对话框
  - 自定义对话框内容
  - 可配置遮罩层（颜色、透明度）
  - 点击外部关闭（可选）
  - 多种对齐方式
  - 9种动画效果
  - 打开/关闭事件
- **使用示例**:
```xml
<ui:DialogHost x:Name="MyDialogHost" CloseOnClickOutside="True" OpenAnimation="ZoomFade">
    <ui:DialogHost.DialogContent>
        <ui:Card Width="400" Padding="24">
            <StackPanel>
                <TextBlock Text="确认删除" FontSize="18" FontWeight="SemiBold" />
                <TextBlock Text="确定要删除吗？" Margin="0,12,0,0" />
                <StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,16,0,0">
                    <Button Content="取消" Click="CancelClick" />
                    <Button Content="删除" Click="ConfirmClick" Margin="8,0,0,0" />
                </StackPanel>
            </StackPanel>
        </ui:Card>
    </ui:DialogHost.DialogContent>
    <Grid>
        <Button Content="显示对话框" Click="ShowDialogClick" />
    </Grid>
</ui:DialogHost>
```

#### MessageBox（消息框）
- **文件位置**: `Controls/Dialogs/MessageBox.cs`
- **完成度**: 100%
- **特性**:
  - 静态便捷方法
  - 多种按钮组合
  - 多种图标类型
  - 复选框支持（"不再提示"）
  - 键盘操作支持（ESC、Enter）
- **使用示例**:
```csharp
MessageBox.Information("这是一条信息");
MessageBox.Success("操作成功！");
MessageBox.Warning("请注意！");
MessageBox.Error("发生错误！");

if (MessageBox.Confirm("确定要继续吗？"))
{
    // 用户点击了"是"
}

var result = MessageBox.Show("确定要删除吗？", "确认", MessageBoxButton.YesNo);
```

#### Snackbar（通知栏）
- **文件位置**: `Controls/Notifications/Snackbar.cs`
- **样式文件**: `Styles/Win11/Win11.Snackbar.xaml`
- **完成度**: 100%
- **特性**:
  - 四种严重程度（Info、Success、Warning、Error）
  - 自动消失（可配置时长）
  - 操作按钮支持
  - 图标和关闭按钮（可选）
  - 事件系统
- **使用示例**:
```xml
<ui:Snackbar x:Name="MySnackbar" VerticalAlignment="Bottom" HorizontalAlignment="Center" />
```
```csharp
MySnackbar.Show("文件已保存成功");
MySnackbar.Show("文件已删除", "撤销", () => { /* 撤销操作 */ });
MySnackbar.Show("操作成功", SnackbarSeverity.Success);
MySnackbar.Show("操作失败", SnackbarSeverity.Error, duration: 0); // 不自动关闭
```

### 7. 导航控件

#### Sidebar（侧边栏）
- **文件位置**: `Controls/Navigation/Sidebar.xaml.cs`
- **完成度**: 95%
- **特性**:
  - 收缩/展开动画（240px ↔ 56px）
  - 搜索功能集成（收缩时显示搜索图标，展开时显示搜索框）
  - 层级导航支持（父子级导航项）
  - 底部导航项（固定在底部的导航按钮）
  - 收缩状态自动折叠子项
  - 自定义导航项样式
  - 流畅的宽度动画过渡
- **使用示例**:
```xml
<ui:Sidebar>
    <ui:Sidebar.DataContext>
        <local:MainViewModel />
    </ui:Sidebar.DataContext>
</ui:Sidebar>
```
```csharp
// ViewModel 需要提供以下属性
public ObservableCollection<NavigationItem> NavigationItems { get; }
public ObservableCollection<NavigationItem> BottomNavigationItems { get; }

// NavigationItem 模型
public class NavigationItem
{
    public string Title { get; set; }
    public string Icon { get; set; }  // Segoe MDL2 Assets 图标
    public string PageKey { get; set; }
    public bool HasChildren { get; set; }
    public bool IsExpanded { get; set; }
    public ObservableCollection<NavigationItem>? Children { get; set; }
    public bool IsEnabled { get; set; } = true;
}
```
- **事件**:
  - `NavigateToPage` - 导航到页面事件（路由事件）
- **样式自定义**:
  - `NavItemStyle` - 展开状态的导航项样式
  - `NavItemCollapsedStyle` - 收缩状态的导航项样式
- **主题资源**:
  - `SidebarBackgroundBrush` - 侧边栏背景
  - `SidebarForegroundBrush` - 侧边栏前景色
  - `SidebarHoverBackgroundBrush` - 悬停背景
  - `SidebarSelectedBackgroundBrush` - 选中背景

#### MainTitleBar（标题栏）
- **文件位置**: `Controls/Chrome/MainTitleBar.xaml.cs`
- **完成度**: 90%
- **特性**:
  - 自定义窗口标题栏
  - 窗口控制按钮（最小化、最大化/还原、关闭）
  - 导航菜单支持
  - 双击标题栏切换最大化状态
  - 拖动窗口功能
  - 最大化状态下拖动还原支持（需窗口支持 `RestoreWindowForDrag` 方法）
- **使用示例**:
```xml
<Window x:Class="MainWindow"
        WindowStyle="None"
        AllowsTransparency="True">
    <Grid>
        <ui:MainTitleBar Title="我的应用" />
    </Grid>
</Window>
```
```csharp
// 设置标题
myTitleBar.Title = "新标题";

// 处理导航事件
myTitleBar.NavigateToPage += (s, e) =>
{
    var pageKey = e.PageKey;
    var pageTitle = e.PageTitle;
    // 执行导航逻辑
};
```
- **事件**:
  - `NavigateToPage` - 导航到页面事件（路由事件）
- **依赖属性**:
  - `Title` - 窗口标题
  - `CanMinimize` - 是否显示最小化按钮（默认: true）
  - `CanMaximize` - 是否显示最大化按钮（默认: true）
  - `CanClose` - 是否显示关闭按钮（默认: true）
- **集成说明**:
  - 需要将 `WindowStyle` 设置为 `None`
  - 建议设置 `AllowsTransparency="True"`
  - 窗口需要实现 `SaveWindowStateBeforeMaximize` 和 `RestoreWindowForDrag` 方法以获得完整功能

#### DocumentTabHost（文档标签页）
- **文件位置**: `Controls/Tabs/DocumentTabHost.xaml.cs`
- **完成度**: 90%
- **特性**:
  - 多标签页管理
  - 标签页拖拽重排
  - 标签页关闭按钮
  - 当前标签高亮
  - 右键菜单（关闭、关闭其他、关闭全部）
  - 内容区域全屏模式
  - 窗口全屏模式（F11快捷键支持）
- **使用示例**:
```xml
<ui:DocumentTabHost x:Name="TabHost" />
```
```csharp
// 打开新页面
var page = new MyPage();
TabHost.OpenPage("页面标题", page, "PageKey");

// 获取当前选中的标签
var selectedTab = TabHost.SelectedTab;

// 关闭所有标签（通过右键菜单或代码）
// TabHost 支持右键菜单操作
```
- **公共方法**:
  - `OpenPage(string title, Page page, string pageKey)` - 打开页面（已存在则聚焦，否则新建）
- **公共属性**:
  - `Tabs` - 标签页集合（ObservableCollection<DocumentTab>）
  - `SelectedTab` - 当前选中的标签
- **右键菜单功能**:
  - 关闭当前标签
  - 关闭其他标签
  - 关闭全部标签
- **全屏功能**:
  - 内容区域全屏（隐藏侧边栏）
  - 窗口全屏（隐藏标题栏和侧边栏，F11快捷键）
- **DocumentTab 模型**:
```csharp
public class DocumentTab
{
    public string Title { get; set; }       // 标签标题
    public Page? Page { get; set; }         // 页面内容
    public string PageKey { get; set; }     // 页面唯一标识
    public bool IsActive { get; set; }      // 是否为当前活动标签
}
```

#### Toolbox（工具箱）
- **文件位置**: `Controls/Toolbox/ToolboxView.xaml.cs`
- **完成度**: 85%
- **特性**:
  - 控件分类展示（按钮、输入、列表、菜单等12个分类）
  - 搜索过滤功能
  - 控件图标和描述显示
  - 样式变体选择（如Button的Primary/Outline等）
  - 自动生成XAML插入代码
  - 支持自定义控件注册
- **使用示例**:
```xml
<ui:ToolboxView />
```
```csharp
// 获取当前选中的控件XAML代码
var xamlCode = toolboxView.GetSelectedXaml();

// 在ViewModel中使用
public class MainViewModel
{
    public ToolboxViewModel Toolbox { get; }

    public MainViewModel()
    {
        Toolbox = new ToolboxViewModel();
    }
}
```
- **ControlRegistry 服务**:
  - 提供所有控件的元数据注册
  - 支持按分类获取控件
  - 支持关键词搜索
  - 可扩展自定义控件
- **ControlInfo 模型**:
```csharp
public class ControlInfo
{
    public string Name { get; set; }              // 控件类名
    public string DisplayName { get; set; }       // 显示名称
    public string Description { get; set; }       // 描述
    public ControlCategory Category { get; set; } // 分类
    public ControlType Type { get; set; }         // 类型（Styled/Custom）
    public string? Icon { get; set; }             // 图标路径
    public string XamlNamespace { get; set; }     // XAML命名空间
    public string SampleCode { get; set; }        // 示例代码
    public List<ControlStyleVariant>? StyleVariants { get; set; } // 样式变体
}
```
- **注册自定义控件**:
```csharp
// 在 ControlRegistry 中添加新控件
private static readonly List<ControlInfo> _controls = new()
{
    // 现有控件...
    new ControlInfo
    {
        Name = "MyCustomControl",
        DisplayName = "我的自定义控件",
        Description = "这是一个自定义控件",
        Category = ControlCategory.Others,
        Type = ControlType.Custom,
        Icon = "M4 4h16v16H4V4z",
        XamlNamespace = "ui",
        XamlNamespaceUri = "clr-namespace:MyApp.Controls;assembly=MyApp",
        SampleCode = "<ui:MyCustomControl />"
    }
};
```
- **支持的控制分类**:
  - Buttons（按钮）
  - Inputs（输入）
  - Lists（列表）
  - Menus（菜单）
  - Progress（进度）
  - Sliders（滑块）
  - Cards（卡片）
  - Dialogs（对话框）
  - Notifications（通知）
  - Navigation（导航）
  - Chrome（窗口装饰）
  - Others（其他）

### 8. 开发者工具控件

#### ScrollBar（滚动条）
- **文件位置**: `Styles/Win11/Win11.ScrollBar.xaml`
- **完成度**: 100%
- **特性**:
  - 现代化的滚动条外观
  - 悬停时显示轨道
  - 平滑的滚动动画

#### Separator（分隔符）
- **文件位置**: `Styles/Win11/Win11.Controls.xaml`
- **完成度**: 100%
- **特性**:
  - 水平/垂直分隔
  - 可配置颜色和间距

#### ProgressBar（进度条）
- **文件位置**: `Styles/Win11/Win11.Controls.xaml`
- **完成度**: 100%
- **特性**:
  - 水平进度条
  - 确定性和不确定性模式
  - 可配置高度和颜色

---

## 控件分类索引

### 按功能分类

#### 📝 输入控件
- TextBox
- PasswordBox
- CheckBox
- RadioButton
- SearchBox
- NumericUpDown
- ComboBox（基础完成）
- ToggleSwitch

#### 🖱️ 按钮控件
- Button
- ToggleButton（基础完成）
- DropDownButton
- Badge

#### 📦 容器控件
- Card
- Expander

#### 📊 数据展示
- ListBox（基础完成）
- ListView（未实现）
- DataGrid（未实现）
- TreeView（未实现）

#### 🔔 通知和对话框
- DialogHost
- MessageBox
- Snackbar
- ToolTip（基础完成）

#### ⏳ 进度和状态
- ProgressBar
- ProgressRing
- Slider（基础完成）

#### 🧭 导航和布局
- Sidebar
- MainTitleBar
- TabControl（未实现）
- SplitView（未实现）
- DocumentTabHost（部分完成）

#### 📅 日期和时间
- DatePicker（未实现）
- TimePicker（未实现）
- Calendar（未实现）

#### 📋 菜单和工具栏
- MenuItem（基础完成）
- MenuBar（部分完成）
- Toolbar（未实现）
- StatusBar（未实现）

---

## 主题系统

### 支持的主题

1. **Light（浅色主题）**
   - 文件: `Themes/Light/Light.xaml`
   - 适合日间使用

2. **Dark（深色主题）**
   - 文件: `Themes/Dark/Dark.xaml`
   - 适合夜间使用

3. **NeonCyberpunk（霓虹赛博朋克）**
   - 文件: `Themes/NeonCyberpunk/NeonCyberpunk.xaml`
   - 高对比度、鲜艳色彩

4. **Aurora（极光）**
   - 文件: `Themes/Aurora/Aurora.xaml`
   - 渐变色彩效果

5. **SunsetTropics（日落热带）**
   - 文件: `Themes/SunsetTropics/SunsetTropics.xaml`
   - 温暖的色调

### 主题资源结构

```
Themes/
├── Base/
│   ├── ColorPalette.xaml      # 颜色基础定义
│   ├── SemanticTokens.xaml    # 语义化颜色令牌
│   └── ComponentBrushes.xaml  # 组件画刷
├── Light/
│   ├── ColorPalette.xaml
│   ├── SemanticTokens.xaml
│   ├── ComponentBrushes.xaml
│   └── Light.xaml
├── Dark/
│   ├── ColorPalette.xaml
│   ├── SemanticTokens.xaml
│   ├── ComponentBrushes.xaml
│   └── Dark.xaml
├── NeonCyberpunk/
│   ├── ColorPalette.xaml
│   ├── SemanticTokens.xaml
│   ├── ComponentBrushes.xaml
│   └── NeonCyberpunk.xaml
├── Aurora/
│   ├── ColorPalette.xaml
│   ├── SemanticTokens.xaml
│   ├── ComponentBrushes.xaml
│   └── Aurora.xaml
└── SunsetTropics/
    ├── ColorPalette.xaml
    ├── SemanticTokens.xaml
    ├── ComponentBrushes.xaml
    └── SunsetTropics.xaml
```

### 语义化颜色令牌

- `semantic.primary.*` - 主色调
- `semantic.success.*` - 成功状态（绿色）
- `semantic.warning.*` - 警告状态（黄色）
- `semantic.error.*` - 错误状态（红色）
- `semantic.info.*` - 信息状态（蓝色）

---

## 待实现控件

### 高优先级

#### 1. DataGrid（数据网格）
**预计工作量**: 5-7天

**功能需求**:
- 虚拟化支持（处理大量数据）
- 排序功能
- 筛选功能
- 分组功能
- 自定义列模板
- 行选择模式（单选/多选）
- 固定列/冻结行
- 单元格编辑
- 数据验证
- 导出功能（Excel、CSV）

**样式需求**:
- 斑马纹行
- 悬停行高亮
- 选中行样式
- 排序指示器
- 筛选图标

**参考**: MaterialDesignInXAML 的 DataGrid 样式

#### 2. TreeView（树形视图）
**预计工作量**: 4-5天

**功能需求**:
- 层级数据绑定
- 节点展开/折叠动画
- 拖拽支持
- 虚拟化
- 自定义节点模板
- 复选框支持
- 节点选择模式
- 懒加载子节点
- 搜索/过滤功能

**样式需求**:
- 展开/折叠图标
- 节点连线
- 悬停效果
- 选中状态
- 焦点状态

#### 3. TabControl（标签控件）
**预计工作量**: 3-4天

**功能需求**:
- 可关闭的标签页
- 标签页拖拽重排
- 标签页滚动
- 标签页分组
- 标签页溢出处理
- 标签页固定
- 标签页隐藏
- 标签页预览

**样式需求**:
- 标签页关闭按钮
- 标签页悬停效果
- 活动标签指示器
- 标签页动画
- 标签页图标

### 中优先级

#### 4. ListView（列表视图）
**预计工作量**: 3-4天

**功能需求**:
- 多种布局模式（列表、网格、带图标）
- 分组支持
- 拖拽重排
- 虚拟化
- 选择模式
- 滚动视图

#### 5. DatePicker（日期选择器）
**预计工作量**: 3-4天

**功能需求**:
- 日历视图
- 日期范围选择
- 黑名单日期
- 格式化显示
- 键盘导航
- 今日快速选择
- 月份/年份快速切换

#### 6. TimePicker（时间选择器）
**预计工作量**: 2-3天

**功能需求**:
- 时/分/秒选择
- 12/24小时制
- 时间间隔设置
- 键盘输入支持
- 时区支持

#### 7. Calendar（日历）
**预计工作量**: 2-3天

**功能需求**:
- 月份视图
- 年份视图
- 日期范围选择
- 特殊日期标记
- 自定义日期模板

#### 8. SplitView（分割视图）
**预计工作量**: 2-3天

**功能需求**:
- 水平/垂直分割
- 可调整的分割比例
- 面板折叠/展开
- 最小/最大宽度限制
- 分割器拖动

#### 9. AutoCompleteBox（自动完成框）
**预计工作量**: 2-3天

**功能需求**:
- 搜索建议
- 自定义项模板
- 异步搜索
- 最小输入长度
- 去重功能
- 高亮匹配文本

### 低优先级

#### 10. MenuBar（菜单栏）
**预计工作量**: 2天

**功能需求**:
- 顶级菜单
- 子菜单
- 菜单分隔符
- 快捷键显示
- 图标支持
- 复选菜单项
- 单选菜单组

#### 11. StatusBar（状态栏）
**预计工作量**: 1-2天

**功能需求**:
- 多个状态项
- 进度显示
- 自定义大小
- 分隔符

#### 12. Toolbar（工具栏）
**预计工作量**: 2天

**功能需求**:
- 按钮组
- 下拉按钮
- 分隔符
- 自定义大小
- 溢出处理

#### 13. RichTextBox（富文本框）
**预计工作量**: 4-5天

**功能需求**:
- 文本格式化
- 撤销/重做
- 复制/粘贴
- 拖拽支持
- 搜索/替换
- 工具栏集成

---

## 优化方向

### 1. 现有控件增强

#### ComboBox 增强
**当前状态**: 基础完成（90%）

**待增强功能**:
- 多选支持
- 搜索功能
- 自定义项模板优化
- 分组显示
- 虚拟化支持
- 清空按钮

#### ListBox 增强
**当前状态**: 基础完成（90%）

**待增强功能**:
- 拖拽重排
- 虚拟化优化
- 分组功能
- 选择模式增强（单选/多选/扩展选择）
- 项模板优化

#### Slider 增强
**当前状态**: 基础完成（90%）

**待增强功能**:
- 刻度显示
- 范围选择（双滑块）
- 垂直方向支持
- 工具提示显示当前值
- 自定义滑块样式

### 2. 动画系统优化

**当前状态**: 基础动画支持

**优化方向**:
- 统一动画缓动函数
- 添加更多动画预设
- 动画性能优化（使用 Composition API）
- 可配置的动画速度（慢、正常、快）
- 减少动画模式（尊重系统设置）

### 3. 无障碍支持

**当前状态**: 基础支持

**优化方向**:
- 完整的 AutomationPeer 支持
- 键盘导航增强
- 屏幕阅读器支持
- 高对比度模式支持
- 焦点指示器优化
- ARIA 属性映射

### 4. 性能优化

**当前状态**: 良好

**优化方向**:
- 虚拟化支持（DataGrid、TreeView、ListView）
- 延迟加载模板
- 减少视觉树深度
- 使用 Freezable 对象优化
- 控件卸载优化
- 内存泄漏检测和修复

### 5. 设计系统完善

**当前状态**: Windows 11 风格

**优化方向**:
- 设计 Token 系统（间距、圆角、阴影）
- 响应式布局支持
- 触控优化
- 动态流畅度（Fluent Design Motion）
- 光线/深度效果
- 声音反馈

### 6. 开发体验优化

**当前状态**: 良好

**优化方向**:
- 智能提示增强
- 设计时支持
- 示例项目完善
- API 文档生成
- 单元测试覆盖
- 集成测试

### 7. 主题系统扩展

**当前状态**: 5 个主题

**优化方向**:
- 主题编辑器
- 自定义主题生成器
- 主题切换动画
- 主题预览
- 用户自定义主题保存
- 主题导入/导出

---

## 开发路线图

### Phase 1: 基础控件 ✅
**状态**: 已完成

**成果**:
- Button, TextBox, PasswordBox, CheckBox, RadioButton
- ScrollBar, Separator, ProgressBar, Slider
- 基础样式和主题系统

### Phase 2: 高级控件 ✅
**状态**: 已完成

**成果**:
- DialogHost, MessageBox, Snackbar
- Badge, ToggleSwitch, DropDownButton
- Card, Expander, SearchBox, NumericUpDown
- ProgressRing

### Phase 3: 数据展示控件 🔄
**状态**: 进行中

**计划**:
1. **DataGrid**（5-7天）
   - 基础 DataGrid 控件
   - 排序、筛选、分组
   - 自定义列模板

2. **TreeView**（4-5天）
   - 层级数据绑定
   - 拖拽支持
   - 自定义节点模板

3. **ListView 增强**（3-4天）
   - 多种布局模式
   - 拖拽重排
   - 虚拟化

### Phase 4: 导航和布局控件 📅
**状态**: 计划中

**计划**:
1. **TabControl**（3-4天）
2. **SplitView**（2-3天）
3. **TreeView 增强**（2-3天）

### Phase 5: 日期时间控件 📅
**状态**: 计划中

**计划**:
1. **DatePicker**（3-4天）
2. **TimePicker**（2-3天）
3. **Calendar**（2-3天）

### Phase 6: 输入增强 📅
**状态**: 计划中

**计划**:
1. **AutoCompleteBox**（2-3天）
2. **ComboBox 增强**（2-3天）
3. **RichTextBox**（4-5天）

### Phase 7: 菜单和工具栏 📅
**状态**: 计划中

**计划**:
1. **MenuBar 完善**（2天）
2. **Toolbar**（2天）
3. **StatusBar**（1-2天）

### Phase 8: 优化和完善 🔮
**状态**: 规划中

**计划**:
1. 动画系统优化
2. 无障碍支持完善
3. 性能优化
4. 单元测试覆盖
5. 文档完善

---

## 文件结构

```
src/UI/Lemoo.UI/
├── Controls/
│   ├── Buttons/
│   │   ├── Badge.cs
│   │   ├── ToggleSwitch.cs
│   │   └── DropDownButton.cs
│   ├── Cards/
│   │   ├── Card.cs
│   │   └── Expander.cs
│   ├── Inputs/
│   │   ├── SearchBox.cs
│   │   └── NumericUpDown.cs
│   ├── Progress/
│   │   └── ProgressRing.cs
│   ├── Dialogs/
│   │   ├── DialogHost.cs
│   │   ├── MessageBox.cs
│   │   └── MessageBoxWindow.xaml
│   ├── Notifications/
│   │   └── Snackbar.cs
│   ├── Navigation/
│   │   └── Sidebar.xaml.cs
│   ├── Chrome/
│   │   └── MainTitleBar.xaml.cs
│   ├── Tabs/
│   │   └── DocumentTabHost.xaml.cs
│   └── Toolbox/
│       └── ToolboxView.xaml.cs
├── Styles/
│   ├── Win11/
│   │   ├── Win11.Button.xaml
│   │   ├── Win11.TextBox.xaml
│   │   ├── Win11.PasswordBox.xaml
│   │   ├── Win11.CheckBox.xaml
│   │   ├── Win11.ScrollBar.xaml
│   │   ├── Win11.Badge.xaml
│   │   ├── Win11.ToggleSwitch.xaml
│   │   ├── Win11.ProgressRing.xaml
│   │   ├── Win11.DropDownButton.xaml
│   │   ├── Win11.NumericUpDown.xaml
│   │   ├── Win11.Card.xaml
│   │   ├── Win11.Expander.xaml
│   │   ├── Win11.SearchBox.xaml
│   │   ├── Win11.DialogHost.xaml
│   │   ├── Win11.Snackbar.xaml
│   │   ├── Win11.Tokens.xaml
│   │   └── Win11.Controls.xaml
│   ├── Design/
│   │   ├── Animations.xaml
│   │   ├── Shadows.xaml
│   │   ├── Spacing.xaml
│   │   └── Typography.xaml
│   └── CommonStyles.xaml
├── Themes/
│   ├── Base/
│   │   ├── ColorPalette.xaml
│   │   ├── SemanticTokens.xaml
│   │   └── ComponentBrushes.xaml
│   ├── Light/
│   ├── Dark/
│   ├── NeonCyberpunk/
│   ├── Aurora/
│   └── SunsetTropics/
├── Models/
│   ├── ControlCategory.cs
│   ├── ControlType.cs
│   └── ControlInfo.cs
├── Services/
│   └── ControlRegistry.cs
├── ViewModels/
│   └── ToolboxViewModel.cs
└── Converters/
    ├── ArcSegmentConverter.cs
    ├── BooleanToVisibilityConverter.cs
    ├── CategoryExpandedConverter.cs
    ├── CategoryToDisplayNameConverter.cs
    ├── EnumToStringConverter.cs
    ├── InverseBooleanToVisibilityConverter.cs
```

---

## 示例页面

项目包含以下示例页面，展示控件的使用方式：

1. **BadgeSamplePage** - 徽章控件示例
2. **ButtonsSamplePage** - 按钮控件示例（ToggleSwitch、DropDownButton）
3. **CardsSamplePage** - 卡片控件示例
4. **DialogsSamplePage** - 对话框示例
5. **InputsSamplePage** - 输入控件示例
6. **ProgressSamplePage** - 进度控件示例
7. **ThemeSamplePage** - 主题切换示例
8. **ToolboxSamplePage** - 控件工具箱示例

---

## 最佳实践

### 1. 控件使用
- 始终使用 `Style` 属性引用预定义样式
- 使用动态资源引用主题颜色（`{DynamicResource ...}`）
- 为控件设置有意义的 `x:Name` 以便于代码访问

### 2. 主题切换
```csharp
// 切换到深色主题
var darkTheme = new ResourceDictionary()
{
    Source = new Uri("/Lemoo.UI;component/Themes/Dark/Dark.xaml", UriKind.Relative)
};
Application.Current.Resources.MergedDictionaries.Clear();
Application.Current.Resources.MergedDictionaries.Add(darkTheme);
```

### 3. 控件样式继承
```xml
<Style x:Key="CustomButtonStyle" BasedOn="{StaticResource Win11.ButtonStyle}" TargetType="Button">
    <Setter Property="Padding" Value="20,10" />
    <Setter Property="FontSize" Value="14" />
</Style>
```

---

## 贡献指南

### 添加新控件

1. 在 `Controls/` 相应目录下创建控件类
2. 实现依赖属性和路由事件
3. 在 `Styles/Win11/` 下创建样式文件
4. 在 `Win11.Controls.xaml` 中合并样式
5. 创建示例页面展示控件
6. 更新本文档

### 代码规范

- 遵循 C# 命名约定
- 所有公共成员添加 XML 文档注释
- 依赖属性使用 `nameof()` 操作符
- 事件使用虚拟的 `On*` 方法

---

## 参考资料

- [Windows 11 设计规范](https://fluent2.microsoft.design/)
- [MaterialDesignInXAML](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)
- [WPF Toolkit](https://github.com/xceedsoftware/wpftoolkit)

---

## 版本历史

### v1.1.0 (2026-01-15)
**新增内容**:
- ✨ 添加 `Sidebar` 控件详细文档（侧边栏导航）
- ✨ 添加 `MainTitleBar` 控件详细文档（自定义标题栏）
- ✨ 添加 `DocumentTabHost` 控件详细文档（文档标签页）
- ✨ 添加 `ToolboxView` 控件详细文档（控件工具箱）
- 📝 更新控件完成度总览表
- 📝 更新导航工具栏目录结构
- 📝 更新文件结构说明
- 📝 添加 Models、Services、ViewModels 文件结构

**优化**:
- 🎨 完善导航控件的使用示例和说明
- 🎨 补充控件的依赖属性和事件说明
- 🎨 添加主题资源说明

### v1.0.0 (2026-01-14)
**初始版本**:
- 🎉 基础控件库完整实现
- 📋 完整的控件文档
- 🎨 5个主题（Light、Dark、NeonCyberpunk、Aurora、SunsetTropics）
- 🔧 开发者工具支持

---

**文档版本**: 1.1.0
**最后更新**: 2026-01-15
**维护者**: Lemoo.UI Team
