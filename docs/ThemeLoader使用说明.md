# ThemeLoader 使用说明

## 概述

`ThemeLoader` 是一个统一的资源加载顺序管理器，解决了手动维护资源加载顺序的问题。

## 资源层次结构

```
1. 设计基线 (DesignBaseline)
   ├── Typography.xaml     字体排版定义
   ├── Spacing.xaml        间距系统定义
   ├── Shadows.xaml        阴影效果定义
   └── Animations.xaml     动画定义

2. 基础主题 (BaseTheme)
   ├── ColorPalette.xaml   基础色板定义
   ├── SemanticTokens.xaml 语义令牌定义
   └── ComponentBrushes.xaml 组件画刷定义

3. 主题覆盖 (ThemeOverride) [可选]
   ├── ColorPalette.xaml   主题色板覆盖
   ├── SemanticTokens.xaml 主题语义令牌覆盖
   └── ComponentBrushes.xaml 主题组件画刷覆盖

4. 控件样式 (ControlStyles)
   └── Win11.Controls.xaml Win11 控件样式合并入口
```

## 使用方法

### 在 ThemeManager 中使用

```csharp
// 创建主题字典
var themeDict = ThemeLoader.CreateThemeDictionary("Dark", isBaseTheme: false);

// 应用主题
app.Resources.MergedDictionaries.Add(themeDict);
```

### 获取资源配置

```csharp
// 获取主题资源配置（不实际加载资源）
var resources = ThemeLoader.GetThemeResources("Dark", isBaseTheme: false);

// 验证资源配置
var (isValid, errors) = ThemeLoader.ValidateThemeResources(resources);
```

### 获取主题统计信息

```csharp
// 获取主题资源统计
var stats = ThemeLoader.GetThemeStatistics("Dark", isBaseTheme: false);
Console.WriteLine(stats);
```

### 检查主题可用性

```csharp
// 检查主题是否可用
bool available = ThemeLoader.IsThemeAvailable("Dark");
```

## 优势

1. **统一管理**：资源加载顺序在一个地方定义，不需要在每个主题文件中重复
2. **类型安全**：使用枚举定义资源层，编译时检查
3. **易于扩展**：添加新主题只需要调用 `GetThemeResources`，不需要手动维护加载顺序
4. **验证支持**：提供 `ValidateThemeResources` 方法验证资源配置的正确性
5. **调试友好**：提供详细的调试输出和统计信息

## 与旧方案的对比

### 旧方案（手动维护）

每个主题入口文件需要手动维护加载顺序：

```xml
<!-- Dark.xaml -->
<ResourceDictionary.MergedDictionaries>
    <!-- 设计基线 -->
    <ResourceDictionary Source="...Typography.xaml" />
    <ResourceDictionary Source="...Spacing.xaml" />
    <ResourceDictionary Source="...Shadows.xaml" />
    <ResourceDictionary Source="...Animations.xaml" />

    <!-- 基础主题 -->
    <ResourceDictionary Source="...Base/ColorPalette.xaml" />
    <ResourceDictionary Source="...Base/SemanticTokens.xaml" />
    <ResourceDictionary Source="...Base/ComponentBrushes.xaml" />

    <!-- 当前主题 -->
    <ResourceDictionary Source="...Dark/ColorPalette.xaml" />
    <ResourceDictionary Source="...Dark/SemanticTokens.xaml" />
    <ResourceDictionary Source="...Dark/ComponentBrushes.xaml" />

    <!-- 控件样式 -->
    <ResourceDictionary Source="...Win11.Controls.xaml" />
</ResourceDictionary.MergedDictionaries>
```

**问题**：
- 每个主题文件都重复相同的加载顺序
- 如果需要调整顺序，需要修改多个文件
- 容易出错，开发者可能记错顺序
- 难以验证加载顺序的正确性

### 新方案（ThemeLoader）

```csharp
// ThemeManager.cs
var themeDict = ThemeLoader.CreateThemeDictionary("Dark", isBaseTheme: false);
app.Resources.MergedDictionaries.Add(themeDict);
```

**优势**：
- 资源加载顺序在一个地方定义
- 添加新主题不需要手动维护顺序
- 编译时类型检查
- 提供验证和统计功能
- 易于调试和文档化

## 测试验证

### 资源层次验证

```csharp
[Fact]
public void ResourceLoadingOrder_MustFollowHierarchy()
{
    var resources = ThemeLoader.GetThemeResources("Dark", isBaseTheme: false);

    // 验证资源层次顺序
    ThemeLoader.ResourceLayer? previousLayer = null;
    foreach (var resource in resources.Resources)
    {
        if (previousLayer.HasValue)
        {
            Assert.True(resource.Layer >= previousLayer.Value);
        }
        previousLayer = resource.Layer;
    }
}
```

### 资源计数验证

```csharp
[Fact]
public void ResourceLayersCount_DarkTheme_HasCorrectLayers()
{
    var resources = ThemeLoader.GetThemeResources("Dark", isBaseTheme: false);

    var designBaselineCount = resources.Resources.Count(r => r.Layer == ThemeLoader.ResourceLayer.DesignBaseline);
    var baseThemeCount = resources.Resources.Count(r => r.Layer == ThemeLoader.ResourceLayer.BaseTheme);
    var themeOverrideCount = resources.Resources.Count(r => r.Layer == ThemeLoader.ResourceLayer.ThemeOverride);
    var controlStylesCount = resources.Resources.Count(r => r.Layer == ThemeLoader.ResourceLayer.ControlStyles);

    Assert.Equal(4, designBaselineCount); // Typography, Spacing, Shadows, Animations
    Assert.Equal(3, baseThemeCount); // ColorPalette, SemanticTokens, ComponentBrushes (Base)
    Assert.Equal(3, themeOverrideCount); // ColorPalette, SemanticTokens, ComponentBrushes (Dark)
    Assert.Equal(1, controlStylesCount); // Win11.Controls
}
```

## 迁移指南

### 现有主题文件

现有的主题入口文件（如 `Dark.xaml`）仍然可以正常工作，但建议迁移到使用 `ThemeLoader`。

### 迁移步骤

1. **保留现有主题文件**：为了向后兼容，现有主题文件保持不变
2. **更新 ThemeManager**：`ThemeManager` 已经更新为使用 `ThemeLoader`
3. **测试验证**：运行应用程序验证主题切换功能正常

### 未来计划

- 逐步废弃主题入口文件（`Base.xaml`, `Dark.xaml` 等）
- 完全依赖 `ThemeLoader` 进行资源加载
- 添加更多验证和诊断功能

## 总结

`ThemeLoader` 提供了一种统一、类型安全、易于维护的方式来管理 WPF 主题资源加载顺序。它解决了手动维护资源加载顺序的问题，使主题系统更加健壮和易于扩展。
