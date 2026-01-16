# Lemoo.UI - 现代化 WPF UI 组件库

## 项目概述

Lemoo.UI 是一个基于 .NET 10 和 WPF 的现代化 UI 组件库，采用 Fluent Design 设计语言，提供丰富的基础控件、主题系统和图标浏览器，帮助开发者快速构建美观、一致的桌面应用程序。

## 技术栈

- **.NET 10 (LTS)** - 长期支持版本
- **C# 13** - 最新语言特性
- **WPF** - Windows Presentation Foundation
- **CommunityToolkit.Mvvm** - 轻量级 MVVM 框架

## 核心特性

### 1. 丰富的控件库
- 按钮控件（Button、IconButton、CircleButton、HyperlinkButton）
- 输入控件（TextBox、ComboBox、DatePicker、CheckBox、RadioButton）
- 容器控件（Card、GroupBox、Expander）
- 导航控件（TabControl、Breadcrumb、TreeView、MenuItem）
- 数据展示控件（DataGrid、ListView、Badge）
- 反馈控件（ProgressBar、ProgressRing、ToolTip、Popup）
- 其他控件（Image、Separator、ScrollViewer）

### 2. 主题系统
- 支持浅色/深色主题切换
- 多种预设主题色（蓝色、紫色、绿色、橙色、红色、青色）
- 运行时动态切换主题

### 3. 图标系统
- 内置丰富的图标库
- 图标浏览器工具，方便查找和使用图标
- 图标支持多种尺寸和颜色

### 4. 响应式设计
- 流式布局
- 自适应窗口大小
- 触摸友好

## 界面预览

### 主窗口 - 示例展示
![示例展示](示例图片/屏幕截图%202026-01-16%20150453.png)

### 图标浏览器
![图标浏览器](示例图片/屏幕截图%202026-01-16%20150516.png)

### 主题切换效果
![主题切换](示例图片/屏幕截图%202026-01-16%20150608.png)

## 快速开始

### 前置要求
- .NET 10 SDK
- Windows 10/11 操作系统
- Visual Studio 2022 或 Rider（推荐）

### 运行 Lemoo.UI.WPF 示例程序

#### 方法一：使用 Visual Studio
1. 打开 `Lemoo.UI.sln` 解决方案
2. 将 `Lemoo.UI.WPF` 设置为启动项目
3. 按 `F5` 或点击"开始调试"按钮运行

#### 方法二：使用命令行
```bash
# 进入 WPF 项目目录
cd src/Lemoo.UI.WPF

# 还原依赖
dotnet restore

# 运行项目
dotnet run
```

#### 方法三：使用 .NET CLI 发布并运行
```bash
# 发布为单文件可执行程序
cd src/Lemoo.UI.WPF
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# 运行发布后的程序
.\bin\Release\net10.0-windows\win-x64\publish\Lemoo.UI.WPF.exe
```

### 在你的项目中使用 Lemoo.UI

#### 1. 添加项目引用
在你的 WPF 项目中引用 Lemoo.UI：
```bash
dotnet add path/to/Lemoo.UI.csproj
```

#### 2. 在 App.xaml 中引入资源字典
```xml
<Application x:Class="YourApp.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- 引入 Lemoo.UI 主题资源 -->
                <ResourceDictionary Source="pack://application:,,,/Lemoo.UI;component/Themes/Generic.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

#### 3. 在窗口中使用控件
```xml
<Window x:Class="YourApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:lemoo="clr-namespace:Lemoo.UI.Controls;assembly=Lemoo.UI">
    <StackPanel Margin="20">
        <!-- 使用 Lemoo UI 控件 -->
        <lemoo:Button Content="点击我" />
        <lemoo:TextBox PlaceholderText="请输入内容..." />
        <lemoo:Card Header="卡片标题">
            <TextBlock Text="卡片内容" />
        </lemoo:Card>
    </StackPanel>
</Window>
```

#### 4. 切换主题
在代码中动态切换主题：
```csharp
using Lemoo.UI.Themes;

// 切换到深色主题
ThemeManager.Current.ChangeTheme(ThemeMode.Dark);

// 切换主题色
ThemeManager.Current.ChangeAccentColor(AccentColor.Purple);
```

或在 XAML 中绑定：
```xml
<StackPanel>
    <Button Content="切换深色模式" Command="{Binding ToggleDarkModeCommand}" />
    <ComboBox ItemsSource="{Binding AccentColors}"
              SelectedItem="{Binding SelectedAccentColor}" />
</StackPanel>
```

## 控件示例

### 按钮
```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <lemoo:Button Content="标准按钮" />
    <lemoo:Button Content="主要按钮" Style="{StaticResource PrimaryButtonStyle}" />
    <lemoo:IconButton IconKind="Home" />
    <lemoo:CircleButton IconKind="Settings" />
</StackPanel>
```

### 卡片
```xml
<lemoo:Card Header="用户信息" Width="300">
    <StackPanel Spacing="10">
        <lemoo:TextBox PlaceholderText="用户名" />
        <lemoo:PasswordBox PlaceholderText="密码" />
        <lemoo:Button Content="登录" HorizontalAlignment="Stretch" />
    </StackPanel>
</lemoo:Card>
```

### 数据网格
```xml
<lemoo:DataGrid ItemsSource="{Binding Users}" AutoGenerateColumns="False">
    <lemoo:DataGrid.Columns>
        <lemoo:DataGridTextColumn Header="姓名" Binding="{Binding Name}" />
        <lemoo:DataGridTextColumn Header="邮箱" Binding="{Binding Email}" />
        <lemoo:DataGridTemplateColumn Header="操作">
            <lemoo:DataGridTemplateColumn.CellTemplate>
                <DataTemplate>
                    <lemoo:Button Content="编辑" Command="{Binding EditCommand}" />
                </DataTemplate>
            </lemoo:DataGridTemplateColumn.CellTemplate>
        </lemoo:DataGridTemplateColumn>
    </lemoo:DataGrid.Columns>
</lemoo:DataGrid>
```

## 主题配置

### 预设主题色
- **Blue** (默认) - 蓝色主题
- **Purple** - 紫色主题
- **Green** - 绿色主题
- **Orange** - 橙色主题
- **Red** - 红色主题
- **Cyan** - 青色主题

### 自定义主题色
```csharp
// 使用自定义颜色
ThemeManager.Current.ChangeAccentColor(Color.FromRgb(255, 0, 128));
```

## 图标使用

### 使用图标浏览器
1. 运行 Lemoo.UI.WPF 示例程序
2. 点击"图标浏览器"选项卡
3. 浏览和搜索可用图标
4. 点击图标复制名称，在代码中使用

### 在代码中使用图标
```xml
<!-- 使用图标按钮 -->
<lemoo:IconButton IconKind="Home" />

<!-- 在按钮中添加图标 -->
<lemoo:Button>
    <StackPanel Orientation="Horizontal">
        <lemoo:Icon IconKind="Save" Margin="0,0,8,0" />
        <TextBlock Text="保存" />
    </StackPanel>
</lemoo:Button>
```

## 项目结构

```
Lemoo.UI/
├── src/
│   ├── Lemoo.UI/              # 核心控件库
│   │   ├── Controls/          # 自定义控件
│   │   ├── Themes/            # 主题资源
│   │   ├── Converters/        # 值转换器
│   │   └── Icons/             # 图标定义
│   ├── Lemoo.UI.WPF/          # 示例应用程序
│   └── Lemoo.UI.Core/         # 核心基础类
└── docs/                      # 文档
```

## 开发路线图

### 已完成
- [x] 基础控件库
- [x] 主题系统
- [x] 图标系统
- [x] 图标浏览器
- [x] 示例程序

### 计划中
- [ ] 更多高级控件（图表、日历等）
- [ ] 动画系统增强
- [ ] 拖放支持
- [ ] 无障碍功能改进
- [ ] 完整的文档和示例

## 贡献指南

欢迎贡献代码和建议！请遵循以下步骤：

1. Fork 本项目
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 创建 Pull Request

## 许可证

本项目采用 MIT 许可证 - 详见 [LICENSE](LICENSE) 文件

## 联系方式

如有问题或建议，欢迎提交 Issue。
