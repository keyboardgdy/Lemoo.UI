using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lemoo.UI.Commands;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 搜索框控件。
    /// </summary>
    /// <remarks>
    /// SearchBox 是一个专门用于搜索的文本框，带有搜索图标和清除按钮。
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;!-- 基础用法 --&gt;
    /// &lt;ui:SearchBox Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}" /&gt;
    ///
    /// &lt;!-- 带水印 --&gt;
    /// &lt;ui:SearchBox
    ///     PlaceholderText="搜索..."
    ///     Text="{Binding SearchText}" /&gt;
    ///
    /// &lt;!-- 带命令 --&gt;
    /// &lt;ui:SearchBox
    ///     SearchCommand="{Binding SearchCommand}"
    ///     SearchCommandParameter="{Binding SearchText}" /&gt;
    /// </code>
    /// </example>
    public class SearchBox : Control
    {
        #region Constructor

        static SearchBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBox),
                new FrameworkPropertyMetadata(typeof(SearchBox)));
        }

        public SearchBox()
        {
            // 默认清除命令
            ClearCommand = new RelayCommand(_ => Clear());
        }

        #endregion

        #region Text 依赖属性

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(SearchBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        /// <summary>
        /// 获取或设置搜索文本。
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchBox = (SearchBox)d;
            searchBox.OnTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        #endregion

        #region PlaceholderText 依赖属性

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register(
                nameof(PlaceholderText),
                typeof(string),
                typeof(SearchBox),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置占位符文本。
        /// </summary>
        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        #endregion

        #region SearchCommand 依赖属性

        public static readonly DependencyProperty SearchCommandProperty =
            DependencyProperty.Register(
                nameof(SearchCommand),
                typeof(ICommand),
                typeof(SearchBox),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置搜索命令。
        /// </summary>
        public ICommand? SearchCommand
        {
            get => (ICommand?)GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
        }

        #endregion

        #region SearchCommandParameter 依赖属性

        public static readonly DependencyProperty SearchCommandParameterProperty =
            DependencyProperty.Register(
                nameof(SearchCommandParameter),
                typeof(object),
                typeof(SearchBox),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置搜索命令参数。
        /// </summary>
        public object? SearchCommandParameter
        {
            get => GetValue(SearchCommandParameterProperty);
            set => SetValue(SearchCommandParameterProperty, value);
        }

        #endregion

        #region ClearCommand 依赖属性

        public static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.Register(
                nameof(ClearCommand),
                typeof(ICommand),
                typeof(SearchBox),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置清除命令。
        /// </summary>
        public ICommand? ClearCommand
        {
            get => (ICommand?)GetValue(ClearCommandProperty);
            set => SetValue(ClearCommandProperty, value);
        }

        #endregion

        #region 事件

        public static readonly RoutedEvent SearchEvent =
            EventManager.RegisterRoutedEvent(
                nameof(Search),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(SearchBox));

        /// <summary>
        /// 在触发搜索时发生。
        /// </summary>
        public event RoutedEventHandler Search
        {
            add => AddHandler(SearchEvent, value);
            remove => RemoveHandler(SearchEvent, value);
        }

        public static readonly RoutedEvent TextChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(TextChanged),
                RoutingStrategy.Bubble,
                typeof(TextChangedEventHandler),
                typeof(SearchBox));

        /// <summary>
        /// 在文本改变时发生。
        /// </summary>
        public event TextChangedEventHandler TextChanged
        {
            add => AddHandler(TextChangedEvent, value);
            remove => RemoveHandler(TextChangedEvent, value);
        }

        #endregion

        #region 方法

        protected virtual void OnTextChanged(string oldText, string newText)
        {
            var args = new TextChangedEventArgs(TextChangedEvent, UndoAction.None);
            RaiseEvent(args);
        }

        protected virtual void OnSearch()
        {
            RaiseEvent(new RoutedEventArgs(SearchEvent, this));

            if (SearchCommand != null && SearchCommand.CanExecute(SearchCommandParameter))
            {
                SearchCommand.Execute(SearchCommandParameter ?? Text);
            }
        }

        public void Clear()
        {
            Text = string.Empty;
        }

        #endregion
    }
}
