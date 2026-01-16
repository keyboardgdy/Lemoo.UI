using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 自动完成文本框控件。
    /// </summary>
    /// <remarks>
    /// AutoCompleteBox 是一个带有自动完成功能的文本框，当用户输入时会显示匹配的建议列表。
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;!-- 基础用法 --&gt;
    /// &lt;ui:AutoCompleteBox
    ///     ItemsSource="{Binding Suggestions}"
    ///     SelectedItem="{Binding SelectedItem}"
    ///     Text="{Binding SearchText}" /&gt;
    ///
    /// &lt;!-- 自定义显示 --&gt;
    /// &lt;ui:AutoCompleteBox
    ///     ItemsSource="{Binding Users}"
    ///     DisplayMemberPath="Name"
    ///     ValueMemberPath="Id"
    ///     Text="{Binding UserName}" /&gt;
    /// </code>
    /// </example>
    public class AutoCompleteBox : Control
    {
        #region Fields

        private Popup? _popup;
        private ListBox? _listBox;
        private TextBox? _textBox;
        private bool _isUpdatingText;

        #endregion

        #region Constructor

        static AutoCompleteBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCompleteBox),
                new FrameworkPropertyMetadata(typeof(AutoCompleteBox)));
        }

        public AutoCompleteBox()
        {
            PreviewKeyDown += AutoCompleteBox_PreviewKeyDown;
            LostFocus += AutoCompleteBox_LostFocus;
        }

        #endregion

        #region Text 依赖属性

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(AutoCompleteBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        /// <summary>
        /// 获取或设置文本框中的文本。
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var autoCompleteBox = (AutoCompleteBox)d;
            if (!autoCompleteBox._isUpdatingText)
            {
                autoCompleteBox.FilterItems();
                autoCompleteBox.OnTextChanged((string)e.OldValue, (string)e.NewValue);
            }
        }

        #endregion

        #region ItemsSource 依赖属性

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(AutoCompleteBox),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置用于显示建议的数据源。
        /// </summary>
        public IEnumerable? ItemsSource
        {
            get => (IEnumerable?)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        #endregion

        #region SelectedItem 依赖属性

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                nameof(SelectedItem),
                typeof(object),
                typeof(AutoCompleteBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

        /// <summary>
        /// 获取或设置选中的项。
        /// </summary>
        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var autoCompleteBox = (AutoCompleteBox)d;
            autoCompleteBox.OnSelectedItemChanged(e.OldValue, e.NewValue);
        }

        #endregion

        #region DisplayMemberPath 依赖属性

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                nameof(DisplayMemberPath),
                typeof(string),
                typeof(AutoCompleteBox),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置用于显示的属性路径。
        /// </summary>
        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        #endregion

        #region ValueMemberPath 依赖属性

        public static readonly DependencyProperty ValueMemberPathProperty =
            DependencyProperty.Register(
                nameof(ValueMemberPath),
                typeof(string),
                typeof(AutoCompleteBox),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置用于获取值的属性路径。
        /// </summary>
        public string ValueMemberPath
        {
            get => (string)GetValue(ValueMemberPathProperty);
            set => SetValue(ValueMemberPathProperty, value);
        }

        #endregion

        #region MinimumPrefixLength 依赖属性

        public static readonly DependencyProperty MinimumPrefixLengthProperty =
            DependencyProperty.Register(
                nameof(MinimumPrefixLength),
                typeof(int),
                typeof(AutoCompleteBox),
                new PropertyMetadata(1));

        /// <summary>
        /// 获取或设置显示建议所需的最小字符数。
        /// </summary>
        public int MinimumPrefixLength
        {
            get => (int)GetValue(MinimumPrefixLengthProperty);
            set => SetValue(MinimumPrefixLengthProperty, value);
        }

        #endregion

        #region MaxDropDownHeight 依赖属性

        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register(
                nameof(MaxDropDownHeight),
                typeof(double),
                typeof(AutoCompleteBox),
                new PropertyMetadata(200.0));

        /// <summary>
        /// 获取或设置下拉列表的最大高度。
        /// </summary>
        public double MaxDropDownHeight
        {
            get => (double)GetValue(MaxDropDownHeightProperty);
            set => SetValue(MaxDropDownHeightProperty, value);
        }

        #endregion

        #region IsDropDownOpen 依赖属性

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(
                nameof(IsDropDownOpen),
                typeof(bool),
                typeof(AutoCompleteBox),
                new PropertyMetadata(false, OnIsDropDownOpenChanged));

        /// <summary>
        /// 获取或设置下拉列表是否打开。
        /// </summary>
        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var autoCompleteBox = (AutoCompleteBox)d;
            if (autoCompleteBox._popup != null)
            {
                autoCompleteBox._popup.IsOpen = (bool)e.NewValue;
            }
        }

        #endregion

        #region WatermarkText 依赖属性

        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.Register(
                nameof(WatermarkText),
                typeof(string),
                typeof(AutoCompleteBox),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置水印文本。
        /// </summary>
        public string WatermarkText
        {
            get => (string)GetValue(WatermarkTextProperty);
            set => SetValue(WatermarkTextProperty, value);
        }

        #endregion

        #region 事件

        public static readonly RoutedEvent TextChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(TextChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(AutoCompleteBox));

        /// <summary>
        /// 在文本改变时发生。
        /// </summary>
        public event RoutedEventHandler TextChanged
        {
            add => AddHandler(TextChangedEvent, value);
            remove => RemoveHandler(TextChangedEvent, value);
        }

        public static readonly RoutedEvent SelectionChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SelectionChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(AutoCompleteBox));

        /// <summary>
        /// 在选中项改变时发生。
        /// </summary>
        public event RoutedEventHandler SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        public static readonly RoutedEvent DropDownOpenedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(DropDownOpened),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(AutoCompleteBox));

        /// <summary>
        /// 在下拉列表打开时发生。
        /// </summary>
        public event RoutedEventHandler DropDownOpened
        {
            add => AddHandler(DropDownOpenedEvent, value);
            remove => RemoveHandler(DropDownOpenedEvent, value);
        }

        public static readonly RoutedEvent DropDownClosedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(DropDownClosed),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(AutoCompleteBox));

        /// <summary>
        /// 在下拉列表关闭时发生。
        /// </summary>
        public event RoutedEventHandler DropDownClosed
        {
            add => AddHandler(DropDownClosedEvent, value);
            remove => RemoveHandler(DropDownClosedEvent, value);
        }

        #endregion

        #region 方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_textBox != null)
            {
                _textBox.TextChanged -= TextBox_TextChanged;
            }

            if (_listBox != null)
            {
                _listBox.SelectionChanged -= ListBox_SelectionChanged;
                _listBox.MouseDoubleClick -= ListBox_MouseDoubleClick;
            }

            _textBox = GetTemplateChild("PART_TextBox") as TextBox;
            _listBox = GetTemplateChild("PART_ListBox") as ListBox;
            _popup = GetTemplateChild("PART_Popup") as Popup;

            if (_textBox != null)
            {
                _textBox.TextChanged += TextBox_TextChanged;
            }

            if (_listBox != null)
            {
                _listBox.SelectionChanged += ListBox_SelectionChanged;
                _listBox.MouseDoubleClick += ListBox_MouseDoubleClick;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterItems();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_listBox?.SelectedItem != null)
            {
                SelectedItem = _listBox.SelectedItem;
                UpdateTextFromItem(_listBox.SelectedItem);
                RaiseEvent(new RoutedEventArgs(SelectionChangedEvent, this));
            }
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_listBox?.SelectedItem != null)
            {
                SelectedItem = _listBox.SelectedItem;
                UpdateTextFromItem(_listBox.SelectedItem);
                IsDropDownOpen = false;
            }
        }

        private void AutoCompleteBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_listBox == null) return;

            switch (e.Key)
            {
                case Key.Enter:
                    if (_listBox.SelectedItem != null)
                    {
                        SelectedItem = _listBox.SelectedItem;
                        UpdateTextFromItem(_listBox.SelectedItem);
                        IsDropDownOpen = false;
                        e.Handled = true;
                    }
                    break;

                case Key.Escape:
                    IsDropDownOpen = false;
                    e.Handled = true;
                    break;

                case Key.Down:
                    if (IsDropDownOpen && _listBox.Items.Count > 0)
                    {
                        if (_listBox.SelectedIndex < _listBox.Items.Count - 1)
                        {
                            _listBox.SelectedIndex++;
                            _listBox.ScrollIntoView(_listBox.SelectedItem);
                        }
                        e.Handled = true;
                    }
                    else if (_listBox.Items.Count > 0)
                    {
                        IsDropDownOpen = true;
                        if (_listBox.SelectedIndex == -1 && _listBox.Items.Count > 0)
                        {
                            _listBox.SelectedIndex = 0;
                        }
                        e.Handled = true;
                    }
                    break;

                case Key.Up:
                    if (IsDropDownOpen && _listBox.SelectedIndex > 0)
                    {
                        _listBox.SelectedIndex--;
                        _listBox.ScrollIntoView(_listBox.SelectedItem);
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void AutoCompleteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // 延迟关闭，允许点击列表项
            if (!IsKeyboardFocusWithin)
            {
                IsDropDownOpen = false;
            }
        }

        protected virtual void OnTextChanged(string oldText, string newText)
        {
            RaiseEvent(new RoutedEventArgs(TextChangedEvent, this));
        }

        protected virtual void OnSelectedItemChanged(object? oldValue, object? newValue)
        {
            if (newValue != null)
            {
                UpdateTextFromItem(newValue);
            }
        }

        private void FilterItems()
        {
            if (_listBox == null || ItemsSource == null) return;

            if (string.IsNullOrWhiteSpace(Text) || Text.Length < MinimumPrefixLength)
            {
                IsDropDownOpen = false;
                return;
            }

            var filtered = ItemsSource.Cast<object>().Where(item =>
            {
                var displayText = GetDisplayText(item);
                return displayText != null &&
                       displayText.IndexOf(Text, StringComparison.OrdinalIgnoreCase) >= 0;
            }).ToList();

            _listBox.ItemsSource = filtered;
            IsDropDownOpen = filtered.Count > 0;

            if (filtered.Count > 0 && _listBox.SelectedIndex == -1)
            {
                _listBox.SelectedIndex = 0;
            }

            RaiseEvent(new RoutedEventArgs(filtered.Count > 0 ? DropDownOpenedEvent : DropDownClosedEvent, this));
        }

        private void UpdateTextFromItem(object item)
        {
            _isUpdatingText = true;
            try
            {
                Text = GetDisplayText(item) ?? string.Empty;
            }
            finally
            {
                _isUpdatingText = false;
            }
        }

        private string? GetDisplayText(object item)
        {
            if (item == null) return null;

            if (!string.IsNullOrEmpty(DisplayMemberPath))
            {
                var property = item.GetType().GetProperty(DisplayMemberPath);
                return property?.GetValue(item)?.ToString();
            }

            return item.ToString();
        }

        #endregion
    }
}
