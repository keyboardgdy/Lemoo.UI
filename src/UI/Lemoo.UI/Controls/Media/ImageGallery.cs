using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Lemoo.UI.Models.Media;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 图片画廊控件，用于展示和浏览图片集合。
    /// </summary>
    /// <remarks>
    /// ImageGallery 是一个精美的图片展示控件，支持网格视图、大图预览、
    /// 图片缩放、旋转、全屏模式、幻灯片播放等功能。
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;!-- 基础用法 --&gt;
    /// &lt;ui:ImageGallery ItemsSource="{Binding Images}" /&gt;
    ///
    /// &lt;!-- 列表视图模式 --&gt;
    /// &lt;ui:ImageGallery
    ///     ItemsSource="{Binding Images}"
    ///     ViewMode="List" /&gt;
    ///
    /// &lt;!-- 自定义配置 --&gt;
    /// &lt;ui:ImageGallery
    ///     ItemsSource="{Binding Images}"
    ///     GridItemSize="200"
    ///     EnableSlideShow="True"
    ///     EnableFullScreen="True" /&gt;
    /// </code>
    /// </example>
    [TemplatePart(Name = PART_ItemsControl, Type = typeof(ItemsControl))]
    [TemplatePart(Name = PART_PreviewPopup, Type = typeof(Popup))]
    [TemplatePart(Name = PART_PreviewImage, Type = typeof(Image))]
    public class ImageGallery : ItemsControl
    {
        #region 常量

        private const string PART_ItemsControl = "PART_ItemsControl";
        private const string PART_PreviewPopup = "PART_PreviewPopup";
        private const string PART_PreviewImage = "PART_PreviewImage";

        #endregion

        #region 字段

        private ItemsControl? _itemsControl;
        private Popup? _previewPopup;
        private Image? _previewImage;
        private int _currentIndex = -1;
        private double _currentZoom = 1.0;
        private double _currentRotation = 0.0;

        #endregion

        #region Constructor

        static ImageGallery()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageGallery),
                new FrameworkPropertyMetadata(typeof(ImageGallery)));
        }

        public ImageGallery()
        {
            ViewMode = ImageGalleryMode.Grid;
            GridItemSize = 200;
            GridSpacing = 8;
            EnableZoom = true;
            EnableRotation = true;
            EnableSlideShow = false;
            EnableFullScreen = true;
            SlideShowInterval = TimeSpan.FromSeconds(3);
        }

        #endregion

        #region ViewMode 依赖属性

        public static readonly DependencyProperty ViewModeProperty =
            DependencyProperty.Register(
                nameof(ViewMode),
                typeof(ImageGalleryMode),
                typeof(ImageGallery),
                new PropertyMetadata(ImageGalleryMode.Grid, OnViewModeChanged));

        /// <summary>
        /// 获取或设置视图模式。
        /// </summary>
        public ImageGalleryMode ViewMode
        {
            get => (ImageGalleryMode)GetValue(ViewModeProperty);
            set => SetValue(ViewModeProperty, value);
        }

        private static void OnViewModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageGallery gallery)
            {
                gallery.UpdateItemsPanel();
            }
        }

        #endregion

        #region GridItemSize 依赖属性

        public static readonly DependencyProperty GridItemSizeProperty =
            DependencyProperty.Register(
                nameof(GridItemSize),
                typeof(double),
                typeof(ImageGallery),
                new PropertyMetadata(200.0));

        /// <summary>
        /// 获取或设置网格项大小。
        /// </summary>
        public double GridItemSize
        {
            get => (double)GetValue(GridItemSizeProperty);
            set => SetValue(GridItemSizeProperty, value);
        }

        #endregion

        #region GridSpacing 依赖属性

        public static readonly DependencyProperty GridSpacingProperty =
            DependencyProperty.Register(
                nameof(GridSpacing),
                typeof(double),
                typeof(ImageGallery),
                new PropertyMetadata(8.0));

        /// <summary>
        /// 获取或设置网格间距。
        /// </summary>
        public double GridSpacing
        {
            get => (double)GetValue(GridSpacingProperty);
            set => SetValue(GridSpacingProperty, value);
        }

        #endregion

        #region EnableZoom 依赖属性

        public static readonly DependencyProperty EnableZoomProperty =
            DependencyProperty.Register(
                nameof(EnableZoom),
                typeof(bool),
                typeof(ImageGallery),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置是否启用缩放功能。
        /// </summary>
        public bool EnableZoom
        {
            get => (bool)GetValue(EnableZoomProperty);
            set => SetValue(EnableZoomProperty, value);
        }

        #endregion

        #region EnableRotation 依赖属性

        public static readonly DependencyProperty EnableRotationProperty =
            DependencyProperty.Register(
                nameof(EnableRotation),
                typeof(bool),
                typeof(ImageGallery),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置是否启用旋转功能。
        /// </summary>
        public bool EnableRotation
        {
            get => (bool)GetValue(EnableRotationProperty);
            set => SetValue(EnableRotationProperty, value);
        }

        #endregion

        #region EnableSlideShow 依赖属性

        public static readonly DependencyProperty EnableSlideShowProperty =
            DependencyProperty.Register(
                nameof(EnableSlideShow),
                typeof(bool),
                typeof(ImageGallery),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否启用幻灯片播放。
        /// </summary>
        public bool EnableSlideShow
        {
            get => (bool)GetValue(EnableSlideShowProperty);
            set => SetValue(EnableSlideShowProperty, value);
        }

        #endregion

        #region SlideShowInterval 依赖属性

        public static readonly DependencyProperty SlideShowIntervalProperty =
            DependencyProperty.Register(
                nameof(SlideShowInterval),
                typeof(TimeSpan),
                typeof(ImageGallery),
                new PropertyMetadata(TimeSpan.FromSeconds(3)));

        /// <summary>
        /// 获取或设置幻灯片播放间隔。
        /// </summary>
        public TimeSpan SlideShowInterval
        {
            get => (TimeSpan)GetValue(SlideShowIntervalProperty);
            set => SetValue(SlideShowIntervalProperty, value);
        }

        #endregion

        #region EnableFullScreen 依赖属性

        public static readonly DependencyProperty EnableFullScreenProperty =
            DependencyProperty.Register(
                nameof(EnableFullScreen),
                typeof(bool),
                typeof(ImageGallery),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置是否启用全屏模式。
        /// </summary>
        public bool EnableFullScreen
        {
            get => (bool)GetValue(EnableFullScreenProperty);
            set => SetValue(EnableFullScreenProperty, value);
        }

        #endregion

        #region ZoomMode 依赖属性

        public static readonly DependencyProperty ZoomModeProperty =
            DependencyProperty.Register(
                nameof(ZoomMode),
                typeof(ImageZoomMode),
                typeof(ImageGallery),
                new PropertyMetadata(ImageZoomMode.Fit));

        /// <summary>
        /// 获取或设置缩放模式。
        /// </summary>
        public ImageZoomMode ZoomMode
        {
            get => (ImageZoomMode)GetValue(ZoomModeProperty);
            set => SetValue(ZoomModeProperty, value);
        }

        #endregion

        #region 事件

        public static readonly RoutedEvent ImageClickEvent =
            EventManager.RegisterRoutedEvent(
                nameof(ImageClick),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ImageGallery));

        /// <summary>
        /// 在图片被点击时发生。
        /// </summary>
        public event RoutedEventHandler ImageClick
        {
            add => AddHandler(ImageClickEvent, value);
            remove => RemoveHandler(ImageClickEvent, value);
        }

        public static readonly RoutedEvent ImageDoubleClickEvent =
            EventManager.RegisterRoutedEvent(
                nameof(ImageDoubleClick),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ImageGallery));

        /// <summary>
        /// 在图片被双击时发生。
        /// </summary>
        public event RoutedEventHandler ImageDoubleClick
        {
            add => AddHandler(ImageDoubleClickEvent, value);
            remove => RemoveHandler(ImageDoubleClickEvent, value);
        }

        #endregion

        #region 方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _itemsControl = GetTemplateChild(PART_ItemsControl) as ItemsControl;
            _previewPopup = GetTemplateChild(PART_PreviewPopup) as Popup;
            _previewImage = GetTemplateChild(PART_PreviewImage) as Image;

            UpdateItemsPanel();
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            _currentIndex = -1;
        }

        private void UpdateItemsPanel()
        {
            var itemsPanelTemplate = new ItemsPanelTemplate();
            FrameworkElementFactory panelFactory;

            switch (ViewMode)
            {
                case ImageGalleryMode.Grid:
                    panelFactory = new FrameworkElementFactory(typeof(WrapPanel));
                    panelFactory.SetValue(WrapPanel.OrientationProperty, Orientation.Horizontal);
                    panelFactory.SetValue(WrapPanel.ItemWidthProperty, GridItemSize);
                    panelFactory.SetValue(WrapPanel.ItemHeightProperty, GridItemSize);
                    break;

                case ImageGalleryMode.List:
                    panelFactory = new FrameworkElementFactory(typeof(StackPanel));
                    panelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
                    break;

                case ImageGalleryMode.Masonry:
                    panelFactory = new FrameworkElementFactory(typeof(WrapPanel));
                    panelFactory.SetValue(WrapPanel.OrientationProperty, Orientation.Horizontal);
                    break;

                default:
                    panelFactory = new FrameworkElementFactory(typeof(WrapPanel));
                    break;
            }

            itemsPanelTemplate.VisualTree = panelFactory;
            ItemsPanel = itemsPanelTemplate;
        }

        /// <summary>
        /// 打开图片预览。
        /// </summary>
        public void OpenPreview(int index)
        {
            if (Items == null || index < 0 || index >= Items.Count)
                return;

            _currentIndex = index;
            _currentZoom = 1.0;
            _currentRotation = 0.0;

            if (_previewPopup != null && _previewImage != null)
            {
                var item = Items[index];
                if (item is ImageGalleryItem galleryItem)
                {
                    _previewImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(galleryItem.ImageSource));
                }

                _previewPopup.IsOpen = true;
                RaiseEvent(new RoutedEventArgs(ImageClickEvent, item));
            }
        }

        /// <summary>
        /// 关闭图片预览。
        /// </summary>
        public void ClosePreview()
        {
            if (_previewPopup != null)
            {
                _previewPopup.IsOpen = false;
            }
        }

        /// <summary>
        /// 放大图片。
        /// </summary>
        public void ZoomIn()
        {
            if (!EnableZoom) return;
            _currentZoom = Math.Min(_currentZoom + 0.1, 5.0);
            ApplyZoom();
        }

        /// <summary>
        /// 缩小图片。
        /// </summary>
        public void ZoomOut()
        {
            if (!EnableZoom) return;
            _currentZoom = Math.Max(_currentZoom - 0.1, 0.1);
            ApplyZoom();
        }

        /// <summary>
        /// 重置缩放。
        /// </summary>
        public void ResetZoom()
        {
            if (!EnableZoom) return;
            _currentZoom = 1.0;
            ApplyZoom();
        }

        /// <summary>
        /// 旋转图片。
        /// </summary>
        public void Rotate(double angle)
        {
            if (!EnableRotation) return;
            _currentRotation += angle;
            ApplyRotation();
        }

        /// <summary>
        /// 下一张图片。
        /// </summary>
        public void NextImage()
        {
            if (Items == null || Items.Count == 0) return;
            _currentIndex = (_currentIndex + 1) % Items.Count;
            OpenPreview(_currentIndex);
        }

        /// <summary>
        /// 上一张图片。
        /// </summary>
        public void PreviousImage()
        {
            if (Items == null || Items.Count == 0) return;
            _currentIndex = (_currentIndex - 1 + Items.Count) % Items.Count;
            OpenPreview(_currentIndex);
        }

        private void ApplyZoom()
        {
            if (_previewImage != null)
            {
                _previewImage.RenderTransformOrigin = new Point(0.5, 0.5);
                _previewImage.RenderTransform = new ScaleTransform(_currentZoom, _currentZoom);
            }
        }

        private void ApplyRotation()
        {
            if (_previewImage != null)
            {
                _previewImage.RenderTransformOrigin = new Point(0.5, 0.5);
                _previewImage.RenderTransform = new RotateTransform(_currentRotation);
            }
        }

        #endregion
    }
}
