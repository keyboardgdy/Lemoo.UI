using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lemoo.UI.Models.Icons;
using Lemoo.UI.WPF.ViewModels.Pages;

namespace Lemoo.UI.WPF.Views.Pages
{
    /// <summary>
    /// IconBrowserPage.xaml 的交互逻辑
    /// </summary>
    public partial class IconBrowserPage : Page
    {
        private readonly IconBrowserPageViewModel _viewModel;

        public IconBrowserPage()
        {
            InitializeComponent();
            _viewModel = new IconBrowserPageViewModel();
            DataContext = _viewModel;

            // 初始化空状态
            ShowEmptyState();
        }

        /// <summary>
        /// 搜索文本变化
        /// </summary>
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && _viewModel != null)
            {
                _viewModel.SearchCommand?.Execute(textBox.Text);
            }
        }

        /// <summary>
        /// 尺寸选择变化
        /// </summary>
        private void OnSizeChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag is string tag && _viewModel != null)
            {
                _viewModel.SetSizeCommand?.Execute(tag);
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
                IconKind = Lemoo.UI.Models.Icons.IconKind.Check,
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
            if (SizePreviewContainer != null)
            {
                SizePreviewContainer.Visibility = Visibility.Collapsed;
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
            if (SizePreviewContainer != null)
            {
                SizePreviewContainer.Visibility = Visibility.Visible;
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

            // 更新尺寸预览
            if (SizePreviewXS != null) SizePreviewXS.IconKind = icon.Kind;
            if (SizePreviewS != null) SizePreviewS.IconKind = icon.Kind;
            if (SizePreviewM != null) SizePreviewM.IconKind = icon.Kind;
            if (SizePreviewL != null) SizePreviewL.IconKind = icon.Kind;

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
                DetailKind.Text = icon.Kind.ToString();
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
                // 使用图标的关键词
                DetailKeywords.ItemsSource = icon.Keywords?.Length > 0
                    ? icon.Keywords
                    : new[] { icon.Name, icon.Category };
            }
        }
    }
}
