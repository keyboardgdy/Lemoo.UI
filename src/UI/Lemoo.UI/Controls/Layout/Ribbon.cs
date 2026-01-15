using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 功能区控件。
    /// </summary>
    /// <remarks>
    /// Ribbon 是一个类似 Microsoft Office 功能区的控件，用于组织命令和工具。
    /// </remarks>
    /// <example>
    /// <code>
    /// <!-- 基础用法 -->
    /// <ui:Ribbon>
    ///     <ui:RibbonTab Header="主页">
    ///         <ui:RibbonGroup Header="剪贴板">
    ///             <Button Content="粘贴" Command="{Binding PasteCommand}"/>
    ///             <Button Content="复制" Command="{Binding CopyCommand}"/>
    ///         </ui:RibbonGroup>
    ///     </ui:RibbonTab>
    ///     <ui:RibbonTab Header="插入">
    ///         <ui:RibbonGroup Header="表格">
    ///             <Button Content="表格" Command="{Binding TableCommand}"/>
    ///         </ui:RibbonGroup>
    ///     </ui:RibbonTab>
    /// </ui:Ribbon>
    /// </code>
    /// </example>
    public class Ribbon : HeaderedItemsControl
    {
        #region Constructor

        static Ribbon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ribbon),
                new FrameworkPropertyMetadata(typeof(Ribbon)));
        }

        public Ribbon()
        {
            this.Loaded += OnLoaded;
        }

        #endregion

        #region SelectedTab 依赖属性

        public static readonly DependencyProperty SelectedTabProperty =
            DependencyProperty.Register(
                nameof(SelectedTab),
                typeof(RibbonTab),
                typeof(Ribbon),
                new PropertyMetadata(null, OnSelectedTabChanged));

        /// <summary>
        /// 获取或设置选中的选项卡。
        /// </summary>
        public RibbonTab? SelectedTab
        {
            get => (RibbonTab?)GetValue(SelectedTabProperty);
            set => SetValue(SelectedTabProperty, value);
        }

        private static void OnSelectedTabChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Ribbon ribbon)
            {
                if (e.OldValue is RibbonTab oldTab)
                {
                    oldTab.IsSelected = false;
                }

                if (e.NewValue is RibbonTab newTab)
                {
                    newTab.IsSelected = true;
                }
            }
        }

        #endregion

        #region Event Handlers

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // 自动选择第一个选项卡
            if (SelectedTab == null && Items.Count > 0)
            {
                if (Items[0] is RibbonTab firstTab)
                {
                    SelectedTab = firstTab;
                }
            }

            // 监听 Items 集合变化
            if (Items is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged += (s, args) =>
                {
                    if (args.Action == NotifyCollectionChangedAction.Add && SelectedTab == null)
                    {
                        if (args.NewItems?[0] is RibbonTab newTab)
                        {
                            SelectedTab = newTab;
                        }
                    }
                };
            }
        }

        #endregion

    }

    /// <summary>
    /// Ribbon 选项卡控件。
    /// </summary>
    public class RibbonTab : HeaderedItemsControl
    {
        #region Constructor

        static RibbonTab()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTab),
                new FrameworkPropertyMetadata(typeof(RibbonTab)));
        }

        public RibbonTab()
        {
        }

        #endregion

        #region IsSelected 依赖属性

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                nameof(IsSelected),
                typeof(bool),
                typeof(RibbonTab),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否选中此选项卡。
        /// </summary>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        #endregion

        #region Event Handlers

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // 查找父级 Ribbon 并设置选中的选项卡
            if (Parent is Ribbon ribbon)
            {
                ribbon.SelectedTab = this;
            }
            else if (TemplatedParent is Ribbon ribbonFromTemplate)
            {
                ribbonFromTemplate.SelectedTab = this;
            }
            else
            {
                // 通过可视化树查找父级 Ribbon
                var parentRibbon = FindParent<Ribbon>(this);
                if (parentRibbon != null)
                {
                    parentRibbon.SelectedTab = this;
                }
            }
        }

        private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            if (parentObject is T parent)
                return parent;

            return FindParent<T>(parentObject);
        }

        #endregion

    }

    /// <summary>
    /// Ribbon 分组控件。
    /// </summary>
    public class RibbonGroup : HeaderedItemsControl
    {
        #region Constructor

        static RibbonGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGroup),
                new FrameworkPropertyMetadata(typeof(RibbonGroup)));
        }

        public RibbonGroup()
        {
        }

        #endregion

        #region Icon 依赖属性

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(object),
                typeof(RibbonGroup),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置分组的图标。
        /// </summary>
        public object? Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        #endregion

    }

    /// <summary>
    /// Ribbon 按钮控件。
    /// </summary>
    public class RibbonButton : Button
    {
        #region Constructor

        static RibbonButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonButton),
                new FrameworkPropertyMetadata(typeof(RibbonButton)));
        }

        public RibbonButton()
        {
        }

        #endregion

        #region Label 依赖属性

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(
                nameof(Label),
                typeof(string),
                typeof(RibbonButton),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置按钮的标签文本。
        /// </summary>
        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        #endregion

        #region Image 依赖属性

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(
                nameof(Image),
                typeof(object),
                typeof(RibbonButton),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置按钮的图像。
        /// </summary>
        public object? Image
        {
            get => GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        #endregion

        #region IsLarge 依赖属性

        public static readonly DependencyProperty IsLargeProperty =
            DependencyProperty.Register(
                nameof(IsLarge),
                typeof(bool),
                typeof(RibbonButton),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否显示为大按钮。
        /// </summary>
        public bool IsLarge
        {
            get => (bool)GetValue(IsLargeProperty);
            set => SetValue(IsLargeProperty, value);
        }

        #endregion

    }

    /// <summary>
    /// Ribbon 分隔符控件。
    /// </summary>
    public class RibbonSeparator : Separator
    {
        #region Constructor

        static RibbonSeparator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonSeparator),
                new FrameworkPropertyMetadata(typeof(RibbonSeparator)));
        }

        public RibbonSeparator()
        {
        }

        #endregion

    }

    /// <summary>
    /// Ribbon 快速访问工具栏控件。
    /// </summary>
    public class QuickAccessToolBar : ToolBar
    {
        #region Constructor

        static QuickAccessToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QuickAccessToolBar),
                new FrameworkPropertyMetadata(typeof(QuickAccessToolBar)));
        }

        public QuickAccessToolBar()
        {
        }

        #endregion

    }
}
