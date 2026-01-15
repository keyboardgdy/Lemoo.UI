# Phase 2 - 高级控件实现总结

本文档记录了 Lemoo.UI 项目 Phase 2 中实现的三个高级控件：DialogHost、Snackbar 和 MessageBox。

## 实现概述

### 1. DialogHost 对话框系统

**文件位置：** `src/UI/Lemoo.UI/Controls/Dialogs/DialogHost.cs`

**功能特性：**
- 在窗口内显示模态对话框，而非打开新窗口
- 支持自定义对话框内容（通过 DialogContent 属性）
- 可配置的遮罩层（颜色、透明度）
- 支持点击外部区域关闭对话框
- 多种对话框对齐方式（水平/垂直）
- 支持多种打开/关闭动画效果
- 完整的事件系统（DialogOpened、DialogClosed）

**使用示例：**
```xml
<ui:DialogHost x:Name="MyDialogHost">
    <ui:DialogHost.DialogContent>
        <ui:Card Width="400" Padding="24">
            <StackPanel>
                <TextBlock Text="确认删除" FontSize="18" FontWeight="SemiBold"/>
                <TextBlock Text="确定要删除这个项目吗？" Margin="0,12,0,0"/>
                <StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,16,0,0">
                    <Button Content="取消" Click="CancelClick"/>
                    <Button Content="删除" Click="ConfirmClick" Margin="8,0,0,0"/>
                </StackPanel>
            </StackPanel>
        </ui:Card>
    </ui:DialogHost.DialogContent>

    <!-- 主内容 -->
    <Grid>
        <Button Content="显示对话框" Click="ShowDialogClick"/>
    </Grid>
</ui:DialogHost>
```

```csharp
// 显示对话框
private void ShowDialogClick(object sender, RoutedEventArgs e)
{
    MyDialogHost.IsOpen = true;
}

// 关闭对话框
private void CancelClick(object sender, RoutedEventArgs e)
{
    MyDialogHost.IsOpen = false;
}
```

**支持的动画类型：**
- `None` - 无动画
- `FadeIn` / `FadeOut` - 淡入淡出
- `SlideFromTop` / `SlideFromBottom` - 上下滑入
- `SlideFromLeft` / `SlideFromRight` - 左右滑入
- `Zoom` - 缩放
- `ZoomFade` - 缩放并淡入

### 2. Snackbar 通知系统

**文件位置：** `src/UI/Lemoo.UI/Controls/Notifications/Snackbar.cs`

**功能特性：**
- 非侵入式的临时消息提示
- 四种严重程度类型（Info、Success、Warning、Error）
- 自动消失机制（可配置时长）
- 支持操作按钮（如"撤销"按钮）
- 可选的关闭按钮
- 平滑的淡入滑入动画
- 事件系统（ActionClick、Closed）

**使用示例：**
```xml
<ui:Snackbar x:Name="MySnackbar"
              VerticalAlignment="Bottom"
              HorizontalAlignment="Center"
              Margin="0,0,0,16"/>
```

```csharp
// 显示简单消息
MySnackbar.Show("文件已保存成功");

// 显示带操作按钮的消息
MySnackbar.Show("文件已删除", "撤销", () =>
{
    // 撤销操作
});

// 显示不同类型的通知
MySnackbar.Show("操作成功", SnackbarSeverity.Success);
MySnackbar.Show("操作失败", SnackbarSeverity.Error);
MySnackbar.Show("警告信息", SnackbarSeverity.Warning);
```

**配置属性：**
- `Message` - 消息文本
- `Severity` - 通知类型（Info/Success/Warning/Error）
- `Duration` - 显示时长（毫秒，0表示不自动关闭）
- `ShowIcon` - 是否显示图标
- `ShowCloseButton` - 是否显示关闭按钮
- `ActionButtonContent` - 操作按钮内容
- `CornerRadius` - 圆角半径
- `MaxWidth` - 最大宽度

### 3. MessageBox 消息框

**文件位置：**
- `src/UI/Lemoo.UI/Controls/Dialogs/MessageBox.cs`
- `src/UI/Lemoo.UI/Controls/Dialogs/MessageBoxWindow.xaml`
- `src/UI/Lemoo.UI/Controls/Dialogs/MessageBoxWindow.xaml.cs`

**功能特性：**
- 类似于标准 MessageBox 的静态方法调用
- 多种按钮组合（OK、OKCancel、YesNo、YesNoCancel）
- 多种图标类型（Information、Warning、Error、Question、None）
- 支持复选框选项（如"不再提示"）
- 现代化的 UI 设计
- 支持 ESC 键关闭
- 支持 Enter 键确认
- 静态便捷方法（Information、Success、Warning、Error、Confirm）

**使用示例：**
```csharp
// 显示简单的信息提示
MessageBox.Show("操作成功完成！");

// 显示带标题的信息提示
MessageBox.Show("文件已保存", "提示");

// 显示确认对话框
var result = MessageBox.Show("确定要删除吗？", "确认", MessageBoxButton.YesNo);
if (result == MessageBoxResult.Yes)
{
    // 用户点击了"是"
}

// 显示错误消息
MessageBox.Show("操作失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);

// 使用便捷方法
MessageBox.Information("这是一条信息");
MessageBox.Success("操作成功！");
MessageBox.Warning("请注意！");
MessageBox.Error("发生错误！");

// 确认对话框（返回 bool）
if (MessageBox.Confirm("确定要继续吗？"))
{
    // 用户点击了"是"
}

// 显示带复选框的消息框
var result = MessageBox.Show("不再提示此消息", "提示",
    MessageBoxButton.OK, MessageBoxImage.Information, "不再提示");
if (result.WasOptionChecked)
{
    // 用户勾选了"不再提示"
}
```

**按钮类型：**
- `OK` - 确定按钮
- `OKCancel` - 确定和取消按钮
- `YesNo` - 是和否按钮
- `YesNoCancel` - 是、否和取消按钮

**图标类型：**
- `None` - 无图标
- `Information` - 信息图标
- `Warning` - 警告图标
- `Error` - 错误图标
- `Question` - 问号图标

## 样式文件

### DialogHost 样式
**文件位置：** `src/UI/Lemoo.UI/Styles/Win11/Win11.DialogHost.xaml`

### Snackbar 样式
**文件位置：** `src/UI/Lemoo.UI/Styles/Win11/Win11.Snackbar.xaml`

**样式变体：**
- `Snackbar.DefaultStyle` - 默认样式
- `Snackbar.Info` - 信息样式
- `Snackbar.Success` - 成功样式
- `Snackbar.Warning` - 警告样式
- `Snackbar.Error` - 错误样式

### 主题系统更新

**语义颜色令牌：** `src/UI/Lemoo.UI/Themes/Base/SemanticTokens.xaml`
添加了语义颜色支持不同严重程度的提示：
- `semantic.success.*` - 成功颜色（绿色）
- `semantic.warning.*` - 警告颜色（黄色）
- `semantic.error.*` - 错误颜色（红色）
- `semantic.info.*` - 信息颜色（蓝色）

**组件画刷：** `src/UI/Lemoo.UI/Themes/Base/ComponentBrushes.xaml`
添加了对应的 SolidColorBrush 资源：
- `Brush.Semantic.Success.*`
- `Brush.Semantic.Warning.*`
- `Brush.Semantic.Error.*`
- `Brush.Semantic.Info.*`

## 设计特点

### 1. 遵循项目规范
- 与现有控件保持一致的设计模式
- 使用依赖属性实现数据绑定
- 完整的 XML 文档注释
- 丰富的事件系统

### 2. 主题系统集成
- 完全支持现有的主题系统
- 使用动态资源引用主题颜色
- 支持所有 6 种预定义主题

### 3. 动画效果
- 使用项目中定义的动画时长
- 平滑的缓动函数
- 符合 Windows 11 设计语言

### 4. 可访问性
- 支持键盘导航（ESC、Enter）
- 清晰的视觉层次
- 合理的默认值

## 下一步建议

### Phase 3 - 数据展示控件

1. **DataGrid 数据网格**
   - 虚拟化支持
   - 排序、筛选、分组
   - 自定义列模板
   - 行选择模式

2. **TreeView 树形视图**
   - 层级数据绑定
   - 拖拽支持
   - 自定义节点模板
   - 展开/折叠动画

3. **ListView 列表视图**
   - 多种布局模式（列表、网格、带图标）
   - 分组支持
   - 拖拽重排

### Phase 4 - 布局控件

1. **SplitContainer 分割容器**
   - 水平/垂直分割
   - 可调整的分割比例
   - 折叠面板

2. **TabControl 标签控件**
   - 可关闭的标签页
   - 标签页拖拽
   - 标签页滚动

3. **Accordion 手风琴**
   - 可展开/折叠的面板
   - 一次只能展开一个
   - 动画效果

### Phase 5 - 高级输入控件

1. **AutoComplete 自动完成**
   - 搜索建议
   - 自定义项模板

2. **ComboBox 增强版**
   - 多选支持
   - 搜索功能

3. **DatePicker 日期选择器**
   - 日历视图
   - 范围选择

### 功能增强

1. **动画系统增强**
   - 更多动画类型
   - 动画过渡类

2. **验证系统**
   - 验证规则
   - 错误提示模板

3. **无障碍支持**
   - AutomationPeer
   - 键盘导航增强
   - 屏幕阅读器支持

## 文件清单

### 控件类
- `src/UI/Lemoo.UI/Controls/Dialogs/DialogHost.cs`
- `src/UI/Lemoo.UI/Controls/Dialogs/MessageBox.cs`
- `src/UI/Lemoo.UI/Controls/Dialogs/MessageBoxWindow.xaml`
- `src/UI/Lemoo.UI/Controls/Dialogs/MessageBoxWindow.xaml.cs`
- `src/UI/Lemoo.UI/Controls/Notifications/Snackbar.cs`

### 样式文件
- `src/UI/Lemoo.UI/Styles/Win11/Win11.DialogHost.xaml`
- `src/UI/Lemoo.UI/Styles/Win11/Win11.Snackbar.xaml`

### 主题文件
- `src/UI/Lemoo.UI/Themes/Base/SemanticTokens.xaml`（已更新）
- `src/UI/Lemoo.UI/Themes/Base/ComponentBrushes.xaml`（已更新）
- `src/UI/Lemoo.UI/Styles/Win11/Win11.Controls.xaml`（已更新）

## 测试建议

1. **DialogHost 测试**
   - 测试不同的对齐方式
   - 测试动画效果
   - 测试点击外部关闭
   - 测试键盘操作

2. **Snackbar 测试**
   - 测试不同的严重程度
   - 测试自动关闭
   - 测试操作按钮
   - 测试多次连续显示

3. **MessageBox 测试**
   - 测试所有按钮组合
   - 测试所有图标类型
   - 测试复选框功能
   - 测试键盘操作

---

**文档版本：** 1.0
**最后更新：** 2026-01-14
**实现阶段：** Phase 2 - 高级控件
