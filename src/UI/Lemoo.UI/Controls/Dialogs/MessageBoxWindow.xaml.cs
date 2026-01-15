using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Lemoo.UI.Controls.Dialogs
{
    /// <summary>
    /// MessageBoxWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBoxWindow : Window
    {
        #region Constructor

        public MessageBoxWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        #endregion

        #region 属性

        public MessageBoxResult Result { get; private set; } = MessageBoxResult.None;

        #endregion

        #region 初始化方法

        internal void Initialize(
            string message,
            string caption,
            MessageBoxButton buttons,
            MessageBoxImage icon,
            string? optionText = null)
        {
            // 设置消息
            MessageTextBlock.Text = message;

            // 设置标题
            TitleTextBlock.Text = string.IsNullOrEmpty(caption) ? "提示" : caption;

            // 设置图标
            SetIcon(icon);

            // 设置按钮
            SetButtons(buttons);

            // 设置复选框
            if (!string.IsNullOrEmpty(optionText))
            {
                OptionCheckBox.Content = optionText;
                OptionCheckBox.Visibility = Visibility.Visible;
            }
        }

        private void SetIcon(MessageBoxImage icon)
        {
            string iconText = icon switch
            {
                MessageBoxImage.Error => "\uE783",      // 错误
                MessageBoxImage.Question => "\uE973",    // 问号
                MessageBoxImage.Warning => "\uE7BA",     // 警告
                MessageBoxImage.Information => "\uE897", // 信息
                _ => "\uE897"                            // 默认信息
            };

            Brush iconBrush = icon switch
            {
                MessageBoxImage.Error => new SolidColorBrush(Color.FromRgb(232, 17, 35)),
                MessageBoxImage.Question => new SolidColorBrush(Color.FromRgb(0, 120, 212)),
                MessageBoxImage.Warning => new SolidColorBrush(Color.FromRgb(255, 140, 0)),
                MessageBoxImage.Information => new SolidColorBrush(Color.FromRgb(0, 120, 212)),
                _ => new SolidColorBrush(Color.FromRgb(0, 120, 212))
            };

            IconTextBlock.Text = iconText;
            IconTextBlock.Foreground = iconBrush;
            IconTextBlock.Visibility = icon == MessageBoxImage.None
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void SetButtons(MessageBoxButton buttons)
        {
            ButtonPanel.Children.Clear();

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    AddButton("确定", MessageBoxResult.OK, isDefault: true);
                    break;

                case MessageBoxButton.OKCancel:
                    AddButton("取消", MessageBoxResult.Cancel);
                    AddButton("确定", MessageBoxResult.OK, isDefault: true);
                    break;

                case MessageBoxButton.YesNo:
                    AddButton("否", MessageBoxResult.No);
                    AddButton("是", MessageBoxResult.Yes, isDefault: true);
                    break;

                case MessageBoxButton.YesNoCancel:
                    AddButton("取消", MessageBoxResult.Cancel);
                    AddButton("否", MessageBoxResult.No);
                    AddButton("是", MessageBoxResult.Yes, isDefault: true);
                    break;
            }
        }

        private void AddButton(string content, MessageBoxResult result, bool isDefault = false)
        {
            var button = new Button
            {
                Content = content,
                Style = FindResource("MessageBoxButtonStyle") as Style,
                IsDefault = isDefault,
                Tag = result
            };

            button.Click += (s, e) =>
            {
                Result = result;
                Close();
            };

            ButtonPanel.Children.Add(button);

            if (isDefault)
            {
                button.Focus();
            }
        }

        #endregion

        #region 事件处理

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // 确保窗口在父窗口中心显示
            if (Owner != null)
            {
                Left = Owner.Left + (Owner.ActualWidth - ActualWidth) / 2;
                Top = Owner.Top + (Owner.ActualHeight - ActualHeight) / 2;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // ESC 键关闭
            if (e.Key == Key.Escape)
            {
                Result = MessageBoxResult.Cancel;
                Close();
            }
            // Enter 键触发默认按钮
            else if (e.Key == Key.Enter)
            {
                foreach (var child in ButtonPanel.Children)
                {
                    if (child is Button button && button.IsDefault)
                    {
                        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        return;
                    }
                }
            }
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 获取复选框的选中状态。
        /// </summary>
        public bool IsOptionChecked => OptionCheckBox.IsChecked == true;

        #endregion
    }
}
