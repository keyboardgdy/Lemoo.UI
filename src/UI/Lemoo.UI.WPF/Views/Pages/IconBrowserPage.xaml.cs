using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Lemoo.UI.Helpers;
using Lemoo.UI.Models.Icons;
using Lemoo.UI.WPF.ViewModels.Pages;

namespace Lemoo.UI.WPF.Views.Pages
{
    /// <summary>
    /// IconBrowserPage.xaml 的交互逻辑
    /// 支持使用新的 IconBrowserPageViewModelV2（基于 IconMetadata.json）
    /// </summary>
    public partial class IconBrowserPage : Page
    {
        private readonly IconBrowserPageViewModelV2 _viewModel;
        private DispatcherTimer? _searchDebounceTimer;

        // 标志：是否使用新的 V2 ViewModel（默认 true）
        private const bool UseV2ViewModel = true;

        public IconBrowserPage()
        {
            InitializeComponent();

            // 使用新的 V2 ViewModel，支持中英文搜索和 IconMetadata.json
            _viewModel = new IconBrowserPageViewModelV2();
            DataContext = _viewModel;

            // 初始化空状态
            ShowEmptyState();

            // 初始化搜索防抖计时器（300ms延迟）
            _searchDebounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _searchDebounceTimer.Tick += OnSearchDebounceTick;

            // 订阅 ListBox 选择变化事件
            if (IconListBox != null)
            {
                IconListBox.SelectionChanged += OnIconSelectionChanged;
            }

            // 订阅主题变化事件，主题切换时刷新图标列表
            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        /// <summary>
        /// 主题变化事件处理
        /// </summary>
        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            // 使用 Dispatcher 确保在UI线程执行
            Dispatcher.BeginInvoke(() =>
            {
                _viewModel?.RefreshIcons();
            }, DispatcherPriority.Loaded);
        }

        /// <summary>
        /// 搜索文本变化（带防抖）
        /// </summary>
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && _viewModel != null)
            {
                // 更新清空按钮显示状态
                UpdateClearButtonVisibility();

                // 重置计时器
                _searchDebounceTimer?.Stop();

                // 如果文本为空，立即执行搜索
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    _viewModel.SearchCommand?.Execute(textBox.Text);
                }
                else
                {
                    // 否则延迟执行搜索
                    _searchDebounceTimer?.Start();
                }
            }
        }

        /// <summary>
        /// 清空搜索按钮点击
        /// </summary>
        private void OnClearSearchClick(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox != null)
            {
                SearchTextBox.Text = string.Empty;
                UpdateClearButtonVisibility();
            }
        }

        /// <summary>
        /// 更新清空按钮的可见性
        /// </summary>
        private void UpdateClearButtonVisibility()
        {
            if (ClearSearchButton != null && SearchTextBox != null)
            {
                ClearSearchButton.Visibility = string.IsNullOrWhiteSpace(SearchTextBox.Text)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        /// <summary>
        /// 防抖计时器触发
        /// </summary>
        private void OnSearchDebounceTick(object? sender, EventArgs e)
        {
            _searchDebounceTimer?.Stop();

            if (SearchTextBox != null && _viewModel != null)
            {
                _viewModel.SearchCommand?.Execute(SearchTextBox.Text);
            }
        }

        /// <summary>
        /// 分类点击
        /// </summary>
        private void OnCategoryClick(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.DataContext is IconCategoryItemViewModel category && _viewModel != null)
            {
                _viewModel.SelectCategoryCommand?.Execute(category);

                // 切换分类后，将滚动位置重置到顶部
                if (IconListBox != null)
                {
                    var scrollViewer = FindVisualChild<ScrollViewer>(IconListBox);
                    if (scrollViewer != null)
                    {
                        scrollViewer.ScrollToTop();
                    }
                }
            }
        }

        /// <summary>
        /// 查找可视化子元素
        /// </summary>
        private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                    return result;

                var resultOfChild = FindVisualChild<T>(child);
                if (resultOfChild != null)
                    return resultOfChild;
            }

            return null;
        }

        /// <summary>
        /// 密度滑块拖动事件
        /// </summary>
        private void OnDensityThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is Thumb thumb && _viewModel != null)
            {
                const double trackWidth = 120.0; // 轨道宽度
                const double thumbWidth = 14.0;  // 滑块宽度

                // 获取当前Margin
                var currentMargin = thumb.Margin.Left;

                // 计算新位置
                var newPosition = currentMargin + e.HorizontalChange;

                // 限制在轨道范围内
                newPosition = Math.Max(0, Math.Min(newPosition, trackWidth - thumbWidth));

                // 转换为密度值 (0-100)
                var newDensity = (newPosition / (trackWidth - thumbWidth)) * 100;

                _viewModel.DensityValue = Math.Clamp(newDensity, 0, 100);
            }
        }

        /// <summary>
        /// 图标选择变化 - 更新详情面板
        /// </summary>
        private void OnIconSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is IconInfo icon)
            {
                UpdateDetailPanel(icon);
            }
        }

        /// <summary>
        /// 复制字形点击
        /// </summary>
        private void OnCopyGlyphClick(object sender, RoutedEventArgs e)
        {
            if (DetailGlyph != null && !string.IsNullOrEmpty(DetailGlyph.Text) && DetailGlyph.Text != "-")
            {
                Clipboard.SetText(DetailGlyph.Text);
                if (sender is Button button)
                {
                    ShowCopyFeedback(button);
                }
            }
        }

        /// <summary>
        /// 复制图标代码点击
        /// </summary>
        private void OnCopyKindClick(object sender, RoutedEventArgs e)
        {
            if (DetailKind != null && !string.IsNullOrEmpty(DetailKind.Text) && DetailKind.Text != "-")
            {
                Clipboard.SetText(DetailKind.Text);
                if (sender is Button button)
                {
                    ShowCopyFeedback(button);
                }
            }
        }

        /// <summary>
        /// 复制 Unicode 点击
        /// </summary>
        private void OnCopyUnicodeClick(object sender, RoutedEventArgs e)
        {
            if (DetailUnicode != null && !string.IsNullOrEmpty(DetailUnicode.Text) && DetailUnicode.Text != "-")
            {
                Clipboard.SetText(DetailUnicode.Text);
                if (sender is Button button)
                {
                    ShowCopyFeedback(button);
                }
            }
        }

        /// <summary>
        /// 显示复制反馈
        /// </summary>
        private void ShowCopyFeedback(Button button)
        {
            if (button == null) return;

            // 保存原始内容
            var originalContent = button.Content;

            // 更改为 Check 图标
            button.Content = new Lemoo.UI.Controls.Icons.Icon
            {
                IconKind = Lemoo.UI.Models.Icons.IconKind.CheckMark,
                IconSize = Lemoo.UI.Models.Icons.IconSize.ExtraSmall
            };

            // 800ms 后恢复原始图标
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = System.TimeSpan.FromMilliseconds(800)
            };
            dispatcherTimer.Tick += (s, e) =>
            {
                button.Content = originalContent;
                dispatcherTimer.Stop();
            };
            dispatcherTimer.Start();
        }

        /// <summary>
        /// 显示空状态
        /// </summary>
        private void ShowEmptyState()
        {
            if (EmptyState != null)
            {
                EmptyState.Visibility = Visibility.Visible;
            }
            if (DetailIcon != null)
            {
                DetailIcon.Visibility = Visibility.Collapsed;
            }
            if (ColorPreviewContainer != null)
            {
                ColorPreviewContainer.Visibility = Visibility.Collapsed;
            }
            if (InfoContainer != null)
            {
                InfoContainer.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 显示图标详情
        /// </summary>
        private void ShowIconDetail()
        {
            if (EmptyState != null)
            {
                EmptyState.Visibility = Visibility.Collapsed;
            }
            if (DetailIcon != null)
            {
                DetailIcon.Visibility = Visibility.Visible;
            }
            if (ColorPreviewContainer != null)
            {
                ColorPreviewContainer.Visibility = Visibility.Visible;
            }
            if (InfoContainer != null)
            {
                InfoContainer.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 更新详情面板
        /// </summary>
        private void UpdateDetailPanel(IconInfo icon)
        {
            ShowIconDetail();

            if (DetailIcon != null)
            {
                DetailIcon.IconKind = icon.Kind;
            }

            // 更新颜色预览
            if (ColorPreviewDark != null) ColorPreviewDark.IconKind = icon.Kind;
            if (ColorPreviewAccent != null) ColorPreviewAccent.IconKind = icon.Kind;
            if (ColorPreviewLight != null) ColorPreviewLight.IconKind = icon.Kind;

            if (DetailName != null)
            {
                DetailName.Text = icon.Name;
            }

            if (DetailCategory != null)
            {
                DetailCategory.Text = icon.Category;
            }

            if (DetailGlyph != null)
            {
                DetailGlyph.Text = icon.Glyph;
            }

            if (DetailKind != null)
            {
                DetailKind.Text = $"IconKind.{icon.Kind}";
            }

            if (DetailUnicode != null)
            {
                // 将字形转换为 Unicode 显示
                if (icon.Glyph.Length > 0)
                {
                    int codePoint = char.ConvertToUtf32(icon.Glyph, 0);
                    DetailUnicode.Text = $"U+{codePoint:X4}";
                }
                else
                {
                    DetailUnicode.Text = "-";
                }
            }

            if (DetailKeywords != null)
            {
                // 使用图标的关键词（现在包含中英文）
                DetailKeywords.ItemsSource = icon.Keywords?.Length > 0
                    ? icon.Keywords.Take(10) // 限制显示前10个
                    : new[] { icon.Name, icon.Category };
            }

            // 更新 ViewModel 的选中图标（用于双向绑定）
            if (_viewModel != null)
            {
                _viewModel.SelectedIcon = icon;
            }
        }
    }
}
