# Lemoo.UI WPF 自定义控件设计文档

> 设计理念：现代化、流畅、高性能、可访问性
> 参考设计：Fluent Design、Material Design 3、macOS Big Sur

---

## 1. 数据展示类控件

### 1.1 CardView - 卡片容器

**设计理念**
现代化的内容容器，支持多种阴影深度、圆角样式和悬停效果。

**核心特性**
- 动态阴影深度（Elevation 0-5）
- 可配置圆角半径（4px - 24px）
- 内置悬停/按压动画
- 支持卡片堆叠和展开模式
- 响应式布局（Grid/Flex模式）

**视觉参数**
```csharp
// 阴影深度
public enum CardElevation {
    Flat = 0,      // 无阴影
    Low = 1,       // 轻微阴影
    Medium = 2,    // 标准阴影
    High = 3,      // 深度阴影
    Float = 4,     // 浮动效果
    Popup = 5      // 弹出效果
}

// 动画时长
HoverAnimationDuration: 200ms
PressAnimationDuration: 100ms
```

**使用场景**
- 仪表板卡片
- 列表项容器
- 设置面板
- 内容分组

---

### 1.2 TimelineView - 时间线控件

**设计理念**
优雅展示时间序列数据，支持垂直/水平布局，丰富的节点样式。

**核心特性**
- 垂直/水平布局切换
- 自定义节点样式（图标、图片、自定义控件）
- 连接线动画（可禁用）
- 节点状态指示（完成/进行中/待处理/错误）
- 支持虚拟化大数据集

**视觉参数**
```csharp
public enum TimelineNodeState {
    Pending,    // 灰色圆点
    InProgress, // 蓝色旋转动画
    Completed,  // 绿色勾选
    Error,      // 红色感叹号
    Warning     // 橙色警告
}

// 节点大小
NodeSize: 24px - 48px

// 连线样式
LineStyle: Solid / Dashed / Dotted / Animated
```

**使用场景**
- 订单跟踪
- 项目里程碑
- 审批流程
- 版本历史

---

### 1.3 StatCard - 统计卡片

**设计理念**
专为数据可视化设计的卡片，集成了趋势指示、迷你图表和进度环。

**核心特性**
- 大数字显示（支持单位、小数格式化）
- 趋势指示器（上升/下降/持平，带颜色和箭头）
- 内置迷你折线图/柱状图
- 环形进度指示器
- 对比数据展示（同比/环比）

**视觉参数**
```csharp
// 趋势类型
public enum TrendDirection {
    Up,       // 绿色，向上箭头
    Down,     // 红色，向下箭头
    Neutral,  // 灰色，横线
    Mixed     // 渐变色
}

// 布局模式
Layout: Compact / Standard / Detailed
```

**使用场景**
- 仪表板统计
- KPI 展示
- 分析报告卡片

---

## 2. 输入类控件

### 2.1 TagInput - 标签输入框

**设计理念**
类似 Gmail 或 Slack 的标签输入体验，支持自动完成、验证和分组。

**核心特性**
- 标签增删动画
- 自动完成建议下拉
- 标签验证（重复、格式、自定义规则）
- 标签分组/分类
- 支持图标/徽章
- 拖拽排序
- 键盘导航（方向键、Backspace、Enter）

**视觉参数**
```csharp
// 标签样式
TagCornerRadius: 12px
TagSpacing: 6px
MaxWidth: Unlimited / Constrained

// 动画
AddAnimation: Scale + Fade (150ms)
RemoveAnimation: Scale out + Fade (200ms)
```

**使用场景**
- 邮件收件人
- 标签/分类输入
- 关键词编辑
- 多选值输入

---

### 2.2 RatingControl - 评分控件

**设计理念**
超越传统星级评分，支持多种图标、分数精度和自定义填充。

**核心特性**
- 多种图标类型（星、心、点赞、表情、自定义）
- 分数精度（整星、半星、0.1精度）
- 填充模式（完全、部分、精确）
- 悬停预览模式
- 文本标签映射（1=很差，5=很好）
- 支持双向绑定分数值

**视觉参数**
```csharp
public enum RatingIconType {
    Star,
    Heart,
    Thumb,
    Emoji,
    Custom
}

// 大小
IconSize: 16px - 48px

// 颜色
InactiveColor: #E0E0E0
ActiveColor: #FFC107
HoverColor: #FFD54F
```

**使用场景**
- 商品评价
- 内容评分
- 满意度调查
- 技能熟练度

---

### 2.3 DatePickerRange - 日期范围选择器

**设计理念**
一体化日期范围选择，视觉化范围高亮，快捷选项。

**核心特性**
- 双日历联动视图
- 范围高亮显示
- 快捷选项（今天、本周、本月、上月、自定义）
- 预设范围下拉
- 最小/最大天数限制
- 工作日/周末高亮
- 节假日标记

**视觉参数**
```csharp
// 快捷选项
PresetRanges: Last 7 Days, Last 30 Days, This Month, Custom

// 范围样式
RangeHighlightColor: Theme Primary (20% opacity)
RangeBorderColor: Theme Primary
SelectedDateColor: Theme Primary
```

**使用场景**
- 报表日期筛选
- 酒店预订
- 活动日程
- 数据导出范围

---

### 2.4 CodeEditor - 代码编辑器

**设计理念**
轻量级代码输入控件，语法高亮、行号、代码折叠。

**核心特性**
- 语法高亮（支持多种语言）
- 行号显示
- 代码折叠
- 自动缩进
- 括号匹配
- 搜索/替换面板
- 主题切换（Dark/Light/High Contrast）

**视觉参数**
```csharp
// 字体
FontFamily: Consolas / JetBrains Mono / Fira Code
FontSize: 12px - 16px

// 主题
Themes: VS Dark, VS Light, Monokai, Solarized

// 行号样式
LineNumberWidth: 50px
LineNumberColor: #858585
CurrentLineHighlight: #1E1E1E
```

**使用场景**
- JSON/XML 编辑器
- SQL 查询编辑器
- 代码片段编辑
- 配置文件编辑

---

## 3. 导航类控件

### 3.1 SidebarNavigation - 侧边栏导航

**设计理念**
可折叠、多级、智能侧边栏，支持图标+文字、仅图标模式。

**核心特性**
- 展开/收起动画（宽度：240px ↔ 64px）
- 多级菜单（支持 3+ 级）
- 徽章计数器
- 图标+文字 ↔ 仅图标模式切换
- 侧边栏位置（左/右）
- 拖拽调整宽度
- 键盘导航
- 搜索框集成

**视觉参数**
```csharp
// 尺寸
ExpandedWidth: 240px
CollapsedWidth: 64px
ItemHeight: 40px

// 动画
CollapseAnimationDuration: 250ms (Cubic Ease)
ExpandAnimationDuration: 250ms (Cubic Ease)

// 颜色
BackgroundColor: #201F1F
HoverColor: #3A3838
ActiveColor: #605E5E
AccentColor: Theme Primary
```

**使用场景**
- 主应用导航
- 管理后台菜单
- 设置页面
- 文档目录

---

### 3.2 BreadcrumbBar - 面包屑导航

**设计理念**
现代化面包屑，支持下拉菜单、图标和自定义分隔符。

**核心特性**
- 自动路由同步
- 下拉快捷跳转（跳过中间层级）
- 图标支持
- 自定义分隔符（/、>、→、-）
- 最大层级限制（超出显示省略号）
- 可点击/禁用状态
- 悬停预览

**视觉参数**
```csharp
// 分隔符
Separator: / (默认)
SeparatorColor: #8A8A8A
SeparatorMargin: 8px

// 节点样式
NodeHoverColor: Theme Primary (10% opacity)
NodeHoverUnderline: true

// 最大显示
MaxVisibleNodes: 5
OverflowText: "..."
```

**使用场景**
- 文件浏览器
- 电商分类导航
- 管理后台层级
- 文档章节导航

---

### 3.3 TabViewEx - 增强标签页视图

**设计理念**
超越原生 TabControl，支持拖拽、关闭、分组、固定。

**核心特性**
- 标签拖拽排序
- 标签关闭按钮（可隐藏）
- 标签固定/取消固定
- 标签分组（颜色标记）
- 标签下拉列表（太多标签时）
- 标签滚动（鼠标滚轮）
- 中键关闭标签
- 标签右键菜单（关闭、关闭其他、关闭右侧）
- 标签图标和徽章

**视觉参数**
```csharp
// 标签尺寸
TabMinWidth: 120px
TabMaxWidth: 200px
TabHeight: 36px

// 紧凑模式
CompactTabWidth: 40px (仅图标)

// 颜色
ActiveTabColor: Theme Background
InactiveTabColor: #2D2D2D
HoverTabColor: #3A3A3A
BorderColor: #3A3A3A

// 固定标签
PinnedIndicator: 点标记
PinnedColor: Theme Primary
```

**使用场景**
- 浏览器标签页
- 编辑器/IDE 标签
- 多文档管理
- 设置分组

---

## 4. 反馈类控件

### 4.1 ToastNotification - 通知提示

**设计理念**
非侵入式通知，支持多种类型、队列管理、位置配置。

**核心特性**
- 类型：Info、Success、Warning、Error
- 位置：Top/Bottom + Left/Center/Right
- 进度条（自动关闭倒计时）
- 操作按钮（撤销、重试、查看详情）
- 通知队列（最多显示 N 条）
- 滑入/滑出动画
- 点击关闭
- 持续时间配置

**视觉参数**
```csharp
// 位置
Positions: TopLeft, TopCenter, TopRight,
           BottomLeft, BottomCenter, BottomRight

// 尺寸
Width: 320px
MaxHeight: 120px
CornerRadius: 8px

// 动画
SlideInAnimation: Translate + Fade (300ms, EaseOut)
SlideOutAnimation: Translate + Fade (200ms, EaseIn)

// 颜色
InfoColor: #0078D4
SuccessColor: #107C10
WarningColor: #FF8C00
ErrorColor: #D13438
```

**使用场景**
- 操作成功/失败提示
- 后台任务通知
- 系统消息
- 日志提醒

---

### 4.2 ProgressDialog - 进度对话框

**设计理念**
美观的进度展示，支持确定/不确定进度、取消操作。

**核心特性**
- 确定进度（百分比、进度条）
- 不确定进度（旋转动画、波纹动画）
- 子状态文本（如"正在下载文件 X/Y"）
- 取消按钮（可禁用）
- 后台模式（不阻塞界面）
- 多任务进度展示
- 预计剩余时间

**视觉参数**
```csharp
// 进度条样式
ProgressBarType: Linear / Circular

// 线性进度条
Height: 4px
CornerRadius: 2px
Animation: Smooth / Segmented

// 圆形进度条
Size: 48px
StrokeThickness: 4px

// 文本
TitleFontSize: 16px
MessageFontSize: 14px
DetailFontSize: 12px (灰色)
```

**使用场景**
- 文件上传/下载
- 数据导入/导出
- 长时间计算
- 批量操作

---

### 4.3 EmptyState - 空状态控件

**设计理念**
优雅展示空数据场景，提供插图、文案和操作引导。

**核心特性**
- 内置插图集（无数据、无搜索结果、无网络、错误等）
- 自定义插图（SVG/图片）
- 标题+描述文案
- 主操作按钮
- 次要操作链接
- 动画效果（淡入、浮动）

**视觉参数**
```csharp
// 预设类型
Presets: NoData, NoSearchResults, NoNetwork,
         Error, Loading, Maintenance

// 尺寸
IllustrationSize: 128px - 256px
MaxWidth: 400px

// 颜色
IllustrationColor: #8A8A8A (30% opacity)
TitleColor: Theme Foreground
DescriptionColor: #8A8A8A
```

**使用场景**
- 空列表/表格
- 搜索无结果
- 页面加载失败
- 功能引导

---

## 5. 布局类控件

### 5.1 SplitViewEx - 增强分割视图

**设计理念**
可拖拽、可嵌套、可折叠的分割面板。

**核心特性**
- 水平/垂直分割
- 拖拽调整大小
- 面板展开/收起按钮
- 最小/最大宽度限制
- 分割器样式（线条、手柄、双箭头）
- 嵌套分割（3+ 面板）
- 记忆用户调整的尺寸
- 双击分割器自动平衡

**视觉参数**
```csharp
// 分割器样式
SplitterThickness: 4px
SplitterHoverThickness: 8px
SplitterColor: Transparent
SplitterHoverColor: Theme Primary

// 手柄样式
HandleType: Line / Handle / DoubleArrow
HandleSize: 40px

// 动画
PaneCollapseAnimation: 200ms
```

**使用场景**
- 代码编辑器（文件列表+编辑器+输出）
- 邮件客户端（文件夹列表+邮件列表+邮件内容）
- 设计工具（画布+属性面板）
- 聊天应用（联系人列表+聊天窗口）

---

### 5.2 GlassPanel - 毛玻璃面板

**设计理念**
实现 Fluent Design 的亚克力（Acrylic）和毛玻璃效果。

**核心特性**
- 背景模糊效果
- 半透明背景色
- 噪点纹理叠加
- 边缘发光效果
- 深色/浅色模式
- 可配置模糊半径

**视觉参数**
```csharp
// 毛玻璃参数
BlurRadius: 32px - 128px
Opacity: 60% - 90%
NoiseOpacity: 3% - 5%

// 颜色
LightBackgroundColor: #FFFFFF (80% opacity)
DarkBackgroundColor: #202020 (80% opacity)
TintColor: Theme Primary (2% opacity)
```

**使用场景**
- 模态对话框背景
- 侧边栏
- 浮动面板
- 导航栏

---

### 5.3 MasonryPanel - 瀑布流面板

**设计理念**
自适应瀑布流布局，支持不同高度的元素。

**核心特性**
- 自动排列元素（按列）
- 响应式列数（根据容器宽度）
- 列间距控制
- 加载更多动画
- 虚拟化支持（大数据集）
- 拖拽重排

**视觉参数**
```csharp
// 列配置
ColumnMinWidth: 200px
ColumnMaxWidth: 400px
ColumnSpacing: 16px
RowSpacing: 16px

// 响应式断点
Breakpoints: {
    Small: 1 column,
    Medium: 2 columns,
    Large: 3 columns,
    XLarge: 4+ columns
}
```

**使用场景**
- 图片画廊
- 卡片瀑布流
- 朋友圈/Pinterest 风格布局
- 动态/通知列表

---

## 6. 列表类控件

### 6.1 DataGridEx - 增强数据网格

**设计理念**
功能强大的数据表格，内置分页、排序、筛选、导出。

**核心特性**
- 列固定（左右固定）
- 行多选（Checkbox）
- 列排序（单列/多列）
- 列筛选（文本/日期/数值筛选器）
- 行详情（展开/收起）
- 单元格编辑（双击/单击）
- 虚拟滚动（10万+ 行）
- 列宽拖拽调整
- 导出功能（Excel/CSV/PDF）
- 状态行（总计、平均值）

**视觉参数**
```csharp
// 表头
HeaderHeight: 40px
HeaderBackgroundColor: #F5F5F5
HeaderHoverColor: #E8E8E8

// 行
RowHeight: 40px (Auto)
AlternatingRowColor: #F9F9F9
HoverRowColor: #E3F2FD (Theme Primary 10%)
SelectedRowColor: #E3F2FD (Theme Primary 20%)

// 单元格
CellPadding: 12px
SeparatorColor: #E0E0E0
SeparatorThickness: 1px
```

**使用场景**
- 数据管理后台
- 财务报表
- 用户列表
- 订单列表

---

### 6.2 VirtualizingListBox - 虚拟化列表

**设计理念**
高性能列表控件，支持复杂布局和大数据集。

**核心特性**
- UI 虚拟化（仅渲染可见项）
- 不同高度的项（VariableSized）
- 列表分组（可折叠）
- 拖拽重排
- 滑动删除（移动端风格）
- 加载更多（滚动到底部）
- 粘性标题（分组时）
- 项动画（进入/离开）

**视觉参数**
```csharp
// 项样式
ItemHeight: Auto / Fixed
ItemMargin: 8px 0
ItemCornerRadius: 8px

// 分组
GroupHeaderHeight: 40px
GroupHeaderColor: #F5F5F5

// 动画
ItemEnterAnimation: SlideIn + Fade (300ms)
ItemRemoveAnimation: SlideOut + Fade (200ms)
```

**使用场景**
- 聊天记录列表
- 邮件列表
- 新闻动态
- 文件列表

---

### 6.3 TreeViewEx - 增强树视图

**设计理念**
现代化的树形结构控件，支持拖拽、多选、虚拟化。

**核心特性**
- 节点展开/收起动画
- 节点拖拽（移动/复制）
- 节点多选（Ctrl+点击）
- 节点复选框（级联选择）
- 节点图标/缩略图
- 节点徽章/状态
- 节点搜索/过滤
- 节点右键菜单
- 虚拟化（大数据树）
- 懒加载子节点

**视觉参数**
```csharp
// 节点样式
NodeHeight: 32px
NodeIndent: 24px per level
NodePadding: 8px
NodeCornerRadius: 4px

// 展开/收起图标
ExpandIcon: Chevron Right (→)
CollapseIcon: Chevron Down (↓)
IconSize: 12px
IconColor: #8A8A8A

// 连接线
ShowLines: true (可选)
LineColor: #E0E0E0
LineStyle: Dotted
```

**使用场景**
- 文件浏览器
- 组织架构图
- 分类树
- 设置导航树

---

## 7. 媒体类控件

### 7.1 ImageGallery - 图片画廊

**设计理念**
精美的图片展示控件，支持缩放、旋转、标注。

**核心特性**
- 图片网格/列表视图
- 大图预览（灯箱效果）
- 图片缩放（滚轮、按钮）
- 图片旋转（90°旋转）
- 全屏模式
- 幻灯片播放
- 图片缩略图导航
- 图片信息面板（Exif）
- 图片标注/绘图
- 拖拽上传图片

**视觉参数**
```csharp
// 网格视图
GridItemSize: 200px x 200px
GridSpacing: 8px
GridCornerRadius: 4px

// 大图预览
BackgroundColor: #000000 (95% opacity)
OverlayBlur: 10px
Animation: Fade (300ms)

// 缩略图
ThumbnailSize: 80px
ThumbnailSpacing: 4px
SelectedThumbnailBorder: 2px, Theme Primary
```

**使用场景**
- 相册应用
- 图片浏览器
- 产品图片展示
- 设计稿预览

---

### 7.2 AudioPlayer - 音频播放器

**设计理念**
优雅的音频播放控件，波形可视化、播放列表。

**核心特性**
- 播放/暂停/停止
- 进度条（可拖拽）
- 音量控制
- 静音切换
- 循环模式（列表/单曲/关闭）
- 播放速度（0.5x - 2x）
- 波形可视化
- 播放列表
- 当前播放高亮
- 拖拽添加音频

**视觉参数**
```csharp
// 进度条
ProgressBarHeight: 4px
ProgressColor: Theme Primary
BufferColor: Theme Primary (30% opacity)
PlayedColor: Theme Primary

// 波形
WaveformColor: Theme Primary
WaveformBarWidth: 2px
WaveformBarSpacing: 1px

// 按钮尺寸
IconButtonSize: 40px
PlayButtonSize: 56px
```

**使用场景**
- 音乐播放器
- 播客应用
- 音频录制/播放
- 语音消息

---

### 7.3 VideoPlayer - 视频播放器

**设计理念**
功能完整的视频播放器，字幕、倍速、画中画。

**核心特性**
- 播放/暂停/停止
- 进度条（带预览缩略图）
- 音量控制
- 全屏切换
- 画中画模式
- 播放速度（0.25x - 2x）
- 字幕选择/自定义
- 画质选择
- 键盘快捷键
- 拖拽加载视频

**视觉参数**
```csharp
// 控制栏
ControlBarHeight: 48px
ControlBarColor: #000000 (80% opacity)
ControlBarAutoHide: 3000ms

// 进度条
ProgressBarHeight: 4px
ProgressColor: Theme Primary
ThumbnailPreviewSize: 160px x 90px

// 字幕
SubtitleFont: 16px
SubtitleColor: #FFFFFF
SubtitleBackgroundColor: #000000 (50% opacity)
SubtitlePosition: Bottom, 80px
```

**使用场景**
- 视频播放应用
- 在线课程
- 视频会议回放
- 视频编辑器

---

## 8. 其他创意控件

### 8.1 ColorPicker - 颜色选择器

**设计理念**
专业的颜色选择工具，色盘、吸管、历史记录。

**核心特性**
- 色盘选择（HSL）
- RGB/HEX 输入
- 颜色吸管（屏幕取色）
- 颜色历史（最近使用）
- 颜色预设（保存常用色）
- 透明度支持
- 对比度检查
- 色盲模拟预览

**视觉参数**
```csharp
// 色盘
ColorWheelSize: 200px
ColorSliderHeight: 16px

// 预设
PresetGridSize: 8 x 8
PresetColorSize: 24px

// 历史记录
HistoryCount: 10
HistoryColorSize: 32px
```

**使用场景**
- 设计工具
- 主题配置
- 图表颜色设置
- 图像编辑

---

### 8.2 Avatar - 头像控件

**设计理念**
美观的头像展示，支持图片、 initials、状态指示。

**核心特性**
- 图片裁剪为圆形/圆角矩形
- Initials 模式（显示姓名首字母）
- 状态指示器（在线/离线/忙碌/勿扰）
- 徽章计数器
- 组头像（最多显示 4 人）
- 悬停显示用户信息
- 尺寸变体（XS/S/M/L/XL）

**视觉参数**
```csharp
// 尺寸
Sizes: {
    XS: 24px,
    S: 32px,
    M: 40px,
    L: 56px,
    XL: 80px
}

// 状态指示器
StatusSize: 25% of Avatar
StatusPosition: Bottom Right
StatusColors: {
    Online: #107C10,
    Offline: #8A8A8A,
    Busy: #D13438,
    Away: #FF8C00
}

// Initials 模式
BackgroundColors: [10种渐变色]
FontColor: #FFFFFF
FontWeight: SemiBold
```

**使用场景**
- 用户头像
- 聊天联系人
- 团队成员展示
- 评论/回复头像

---

### 8.3 OTPInput - 验证码输入框

**设计理念**
专为 OTP/验证码设计的输入框，自动聚焦、粘贴支持。

**核心特性**
- 固定长度（通常 4-6 位）
- 单独输入框或单个输入框
- 自动聚焦下一个
- 自动聚焦上一个（Backspace）
- 粘贴支持（自动填充）
- 完成后自动提交
- 倒计时重发
- 错误状态抖动动画

**视觉参数**
```csharp
// 单独输入框
InputBoxSize: 48px x 48px
InputBoxSpacing: 8px
InputBoxCornerRadius: 8px
FontSize: 24px
FontWeight: Medium

// 单个输入框
LetterSpacing: 12px
Width: Full + Padding

// 颜色
NormalColor: #E0E0E0
FocusedColor: Theme Primary (30% opacity)
FilledColor: Theme Primary (20% opacity)
ErrorColor: #D13438 (30% opacity)
```

**使用场景**
- 登录验证码
- 双因素认证
- 手机号验证
- 支付验证

---

### 8.4 SwitchEx - 增强开关控件

**设计理念**
现代化的开关控件，支持自定义样式、加载状态、图标。

**核心特性**
- 开/关状态动画
- 加载状态（旋转动画）
- 禁用状态（半透明）
- 自定义颜色（开/关）
- 图标支持（开/关显示图标）
- 标签文本（On/Off 文字）
- 尺寸变体（S/M/L）

**视觉参数**
```csharp
// 尺寸
Sizes: {
    S: 32px x 18px,
    M: 40px x 22px,
    L: 48px x 26px
}

// 动画
AnimationDuration: 200ms (EaseInOut)

// 颜色
OffColor: #8A8A8A
OnColor: Theme Primary
DisabledColor: #E0E0E0

// 滑块
ThumbSize: 14px - 20px
ThumbColor: #FFFFFF
ThumbShadow: 0 1px 3px rgba(0,0,0,0.3)
```

**使用场景**
- 设置开关
- 功能启用/禁用
- 通知设置
- 主题切换

---

### 8.5 CountdownTimer - 倒计时控件

**设计理念**
视觉化倒计时，环形进度、数字动画。

**核心特性**
- 环形进度显示
- 数字动画（滚动效果）
- 天/时/分/秒显示
- 开始/暂停/重置
- 结束回调
- 自定义结束时间格式
- 紧凑模式（仅数字）

**视觉参数**
```csharp
// 环形进度
ProgressSize: 120px - 200px
ProgressThickness: 8px
ProgressColor: Theme Primary
BackgroundColor: #E0E0E0

// 数字
NumberFontSize: 32px - 48px
NumberFontWeight: Bold
NumberColor: Theme Foreground
LabelColor: #8A8A8A

// 动画
NumberAnimationDuration: 500ms
```

**使用场景**
- 限时优惠
- 考试倒计时
- 活动倒计时
- 计时器应用

---

### 8.6 SignaturePad - 签名板

**设计理念**
流畅的手写签名控件，支持压感、保存为图片。

**核心特性**
- 平滑笔触（贝塞尔曲线）
- 压感支持（根据速度调整粗细）
- 笔触颜色/粗细
- 撤销/重做
- 清空画布
- 保存为图片（PNG/SVG）
- 透明背景
- 触摸/鼠标支持

**视觉参数**
```csharp
// 画布
CanvasBackgroundColor: #FFFFFF (or Transparent)
CanvasBorderColor: #E0E0E0
CanvasBorderThickness: 1px
CanvasCornerRadius: 4px

// 笔触
StrokeColor: #000000
StrokeWidth: 2px - 4px
StrokeSmoothing: 4 (贝塞尔曲线平滑度)

// 最小尺寸
MinWidth: 300px
MinHeight: 150px
```

**使用场景**
- 签名确认
- 手写批注
- 绘图板
- 表单签名

---

### 8.7 Carousel - 轮播控件

**设计理念**
流畅的轮播展示，支持多种过渡效果、自动播放。

**核心特性**
- 过渡效果（滑动、淡入淡出、缩放、立方体）
- 自动播放（可配置间隔）
- 指示器（点/数字/缩略图）
- 前进/后退按钮
- 触摸滑动
- 无限循环
- 懒加载图片
- 键盘导航

**视觉参数**
```csharp
// 尺寸
Width: Auto / Fixed
Height: Auto / Fixed
MaxWidth: 1200px
AspectRatio: 16:9 / 4:3 / 21:9

// 指示器
IndicatorType: Dot / Number / Thumbnail
IndicatorSize: 8px - 12px
IndicatorColor: #FFFFFF (50% opacity)
ActiveIndicatorColor: #FFFFFF

// 导航按钮
ButtonSize: 40px
ButtonColor: #FFFFFF
ButtonBackgroundColor: #000000 (30% opacity)
ButtonHoverBackgroundColor: #000000 (50% opacity)

// 动画
TransitionDuration: 500ms - 1000ms
EasingFunction: EaseInOutCubic
```

**使用场景**
- 产品展示
- 图片轮播
- 功能介绍
- 新闻轮播

---

### 8.8 ChartControl - 图表控件

**设计理念**
简洁美观的图表控件，支持多种图表类型。

**核心特性**
- 图表类型：折线图、柱状图、饼图、环形图、面积图、散点图
- 多数据系列
- 图例显示/隐藏
- 数据点工具提示
- 数据点标记
- 坐标轴配置
- 动画加载
- 导出为图片
- 响应式尺寸

**视觉参数**
```csharp
// 颜色
DefaultColors: [Theme Primary, Secondary, ...]
GridColor: #E0E0E0
AxisColor: #8A8A8A

// 折线图
LineWidth: 2px - 4px
PointMarkerSize: 4px - 8px
AreaOpacity: 20% - 50%

// 柱状图
BarWidth: Auto / Fixed
BarSpacing: 4px
BarCornerRadius: 4px

// 动画
AnimationDuration: 800ms
AnimationDelay: 100ms (staggered)
```

**使用场景**
- 数据可视化
- 分析报表
- 统计图表
- 实时监控

---

## 9. 设计原则总结

### 9.1 一致性原则
- 统一的圆角半径（4px/8px/12px）
- 统一的间距（4px 倍数：4px、8px、12px、16px、24px、32px）
- 统一的颜色系统（主题色 + 语义色）
- 统一的动画时长（150ms/200ms/300ms/500ms）

### 9.2 动效设计
- 所有状态变化都应有过渡动画
- 动画时长：快速操作（100-200ms）、一般操作（300ms）、复杂操作（500ms）
- 缓动函数：EaseOut（进入）、EaseIn（离开）、EaseInOut（切换）
- 避免过度动画，保持流畅自然

### 9.3 可访问性
- 支持高对比度模式
- 键盘导航支持（Tab、方向键、Enter、Esc）
- 屏幕阅读器支持（AutomationProperties）
- 触摸目标最小尺寸（44px x 44px）
- 颜色对比度符合 WCAG 2.1 AA 标准

### 9.4 性能优化
- 列表控件支持虚拟化
- 动画使用硬件加速（RenderOptions.CacheInvalidation）
- 延迟加载和懒加载
- 图片缩略图和渐进加载
- 减少不必要的重绘和布局计算

### 9.5 主题支持
- 浅色/深色模式
- 自定义主题色
- 高对比度模式
- 动态主题切换（无需重启）

---

## 10. 实现优先级建议

### 阶段 1：核心控件（高优先级）
1. CardView
2. TagInput
3. ToastNotification
4. ProgressDialog
5. SidebarNavigation

### 阶段 2：常用控件（中优先级）
1. DataGridEx
2. TreeViewEx
3. SplitViewEx
4. RatingControl
5. EmptyState

### 阶段 3：高级控件（低优先级）
1. TimelineView
2. CodeEditor
3. ImageGallery
4. Carousel
5. ChartControl

---

*设计文档版本：1.0*
*最后更新：2026-01-16*
