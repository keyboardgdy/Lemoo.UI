using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Lemoo.UI.Models.Media;

namespace Lemoo.UI.Controls
{
    /// <summary>
    /// 轮播控件，用于展示图片或内容轮播。
    /// </summary>
    /// <remarks>
    /// Carousel 是一个流畅的轮播展示控件，支持多种过渡效果、自动播放、
    /// 指示器、前进/后退按钮等功能。
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;!-- 基础用法 --&gt;
    /// &lt;ui:Carousel ItemsSource="{Binding Images}" /&gt;
    ///
    /// &lt;!-- 自动播放 + 指示器 --&gt;
    /// &lt;ui:Carousel
    ///     ItemsSource="{Binding Images}"
    ///     AutoPlay="True"
    ///     Interval="0:0:3"
    ///     ShowIndicators="True" /&gt;
    ///
    /// &lt;!-- 自定义过渡效果 --&gt;
    /// &lt;ui:Carousel
    ///     ItemsSource="{Binding Images}"
    ///     Transition="Fade"
    ///     TransitionDuration="0:0:0.5" /&gt;
    /// </code>
    /// </example>
    [TemplatePart(Name = PART_ItemsControl, Type = typeof(ItemsControl))]
    [TemplatePart(Name = PART_PreviousButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_NextButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_IndicatorsPanel, Type = typeof(Panel))]
    public class Carousel : ItemsControl
    {
        #region 常量

        private const string PART_ItemsControl = "PART_ItemsControl";
        private const string PART_PreviousButton = "PART_PreviousButton";
        private const string PART_NextButton = "PART_NextButton";
        private const string PART_IndicatorsPanel = "PART_IndicatorsPanel";

        #endregion

        #region 字段

        private ItemsControl? _itemsControl;
        private Button? _previousButton;
        private Button? _nextButton;
        private Panel? _indicatorsPanel;
        private DispatcherTimer? _autoPlayTimer;
        private int _currentIndex = 0;

        #endregion

        #region Constructor

        static Carousel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Carousel),
                new FrameworkPropertyMetadata(typeof(Carousel)));
        }

        public Carousel()
        {
            AutoPlay = false;
            Interval = TimeSpan.FromSeconds(3);
            ShowNavigationButtons = true;
            ShowIndicators = true;
            Transition = CarouselTransition.Slide;
            TransitionDuration = TimeSpan.FromMilliseconds(500);
            IndicatorType = CarouselIndicatorType.Dot;
            EnableLoop = true;
        }

        #endregion

        #region AutoPlay 依赖属性

        public static readonly DependencyProperty AutoPlayProperty =
            DependencyProperty.Register(
                nameof(AutoPlay),
                typeof(bool),
                typeof(Carousel),
                new PropertyMetadata(false, OnAutoPlayChanged));

        /// <summary>
        /// 获取或设置是否自动播放。
        /// </summary>
        public bool AutoPlay
        {
            get => (bool)GetValue(AutoPlayProperty);
            set => SetValue(AutoPlayProperty, value);
        }

        private static void OnAutoPlayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Carousel carousel)
            {
                carousel.UpdateAutoPlay();
            }
        }

        #endregion

        #region Interval 依赖属性

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                nameof(Interval),
                typeof(TimeSpan),
                typeof(Carousel),
                new PropertyMetadata(TimeSpan.FromSeconds(3), OnIntervalChanged));

        /// <summary>
        /// 获取或设置自动播放间隔。
        /// </summary>
        public TimeSpan Interval
        {
            get => (TimeSpan)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Carousel carousel)
            {
                carousel.UpdateAutoPlay();
            }
        }

        #endregion

        #region ShowNavigationButtons 依赖属性

        public static readonly DependencyProperty ShowNavigationButtonsProperty =
            DependencyProperty.Register(
                nameof(ShowNavigationButtons),
                typeof(bool),
                typeof(Carousel),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置是否显示导航按钮。
        /// </summary>
        public bool ShowNavigationButtons
        {
            get => (bool)GetValue(ShowNavigationButtonsProperty);
            set => SetValue(ShowNavigationButtonsProperty, value);
        }

        #endregion

        #region ShowIndicators 依赖属性

        public static readonly DependencyProperty ShowIndicatorsProperty =
            DependencyProperty.Register(
                nameof(ShowIndicators),
                typeof(bool),
                typeof(Carousel),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置是否显示指示器。
        /// </summary>
        public bool ShowIndicators
        {
            get => (bool)GetValue(ShowIndicatorsProperty);
            set => SetValue(ShowIndicatorsProperty, value);
        }

        #endregion

        #region Transition 依赖属性

        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.Register(
                nameof(Transition),
                typeof(CarouselTransition),
                typeof(Carousel),
                new PropertyMetadata(CarouselTransition.Slide));

        /// <summary>
        /// 获取或设置过渡效果。
        /// </summary>
        public CarouselTransition Transition
        {
            get => (CarouselTransition)GetValue(TransitionProperty);
            set => SetValue(TransitionProperty, value);
        }

        #endregion

        #region TransitionDuration 依赖属性

        public static readonly DependencyProperty TransitionDurationProperty =
            DependencyProperty.Register(
                nameof(TransitionDuration),
                typeof(TimeSpan),
                typeof(Carousel),
                new PropertyMetadata(TimeSpan.FromMilliseconds(500)));

        /// <summary>
        /// 获取或设置过渡动画时长。
        /// </summary>
        public TimeSpan TransitionDuration
        {
            get => (TimeSpan)GetValue(TransitionDurationProperty);
            set => SetValue(TransitionDurationProperty, value);
        }

        #endregion

        #region IndicatorType 依赖属性

        public static readonly DependencyProperty IndicatorTypeProperty =
            DependencyProperty.Register(
                nameof(IndicatorType),
                typeof(CarouselIndicatorType),
                typeof(Carousel),
                new PropertyMetadata(CarouselIndicatorType.Dot));

        /// <summary>
        /// 获取或设置指示器类型。
        /// </summary>
        public CarouselIndicatorType IndicatorType
        {
            get => (CarouselIndicatorType)GetValue(IndicatorTypeProperty);
            set => SetValue(IndicatorTypeProperty, value);
        }

        #endregion

        #region EnableLoop 依赖属性

        public static readonly DependencyProperty EnableLoopProperty =
            DependencyProperty.Register(
                nameof(EnableLoop),
                typeof(bool),
                typeof(Carousel),
                new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置是否启用循环播放。
        /// </summary>
        public bool EnableLoop
        {
            get => (bool)GetValue(EnableLoopProperty);
            set => SetValue(EnableLoopProperty, value);
        }

        #endregion

        #region CurrentIndex 依赖属性

        public static readonly DependencyProperty CurrentIndexProperty =
            DependencyProperty.Register(
                nameof(CurrentIndex),
                typeof(int),
                typeof(Carousel),
                new PropertyMetadata(0, OnCurrentIndexChanged));

        /// <summary>
        /// 获取或设置当前显示项的索引。
        /// </summary>
        public int CurrentIndex
        {
            get => (int)GetValue(CurrentIndexProperty);
            set => SetValue(CurrentIndexProperty, value);
        }

        private static void OnCurrentIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Carousel carousel)
            {
                carousel._currentIndex = (int)e.NewValue;
                carousel.UpdateIndicators();
                carousel.ApplyTransition();
            }
        }

        #endregion

        #region 事件

        public static readonly RoutedEvent SelectionChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SelectionChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(Carousel));

        /// <summary>
        /// 在选择项改变时发生。
        /// </summary>
        public event RoutedEventHandler SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        #endregion

        #region 方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_previousButton != null)
            {
                _previousButton.Click -= PreviousButton_Click;
            }

            if (_nextButton != null)
            {
                _nextButton.Click -= NextButton_Click;
            }

            _itemsControl = GetTemplateChild(PART_ItemsControl) as ItemsControl;
            _previousButton = GetTemplateChild(PART_PreviousButton) as Button;
            _nextButton = GetTemplateChild(PART_NextButton) as Button;
            _indicatorsPanel = GetTemplateChild(PART_IndicatorsPanel) as Panel;

            if (_previousButton != null)
            {
                _previousButton.Click += PreviousButton_Click;
            }

            if (_nextButton != null)
            {
                _nextButton.Click += NextButton_Click;
            }

            UpdateIndicators();
            UpdateAutoPlay();
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            _currentIndex = 0;
            UpdateIndicators();
        }

        private void UpdateAutoPlay()
        {
            if (_autoPlayTimer != null)
            {
                _autoPlayTimer.Stop();
                _autoPlayTimer.Tick -= AutoPlayTimer_Tick;
            }

            if (AutoPlay)
            {
                _autoPlayTimer = new DispatcherTimer
                {
                    Interval = Interval
                };
                _autoPlayTimer.Tick += AutoPlayTimer_Tick;
                _autoPlayTimer.Start();
            }
        }

        private void AutoPlayTimer_Tick(object? sender, EventArgs e)
        {
            Next();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            Previous();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        /// <summary>
        /// 上一张。
        /// </summary>
        public void Previous()
        {
            if (Items == null || Items.Count == 0)
                return;

            if (EnableLoop)
            {
                CurrentIndex = (_currentIndex - 1 + Items.Count) % Items.Count;
            }
            else
            {
                CurrentIndex = Math.Max(0, _currentIndex - 1);
            }
        }

        /// <summary>
        /// 下一张。
        /// </summary>
        public void Next()
        {
            if (Items == null || Items.Count == 0)
                return;

            if (EnableLoop)
            {
                CurrentIndex = (_currentIndex + 1) % Items.Count;
            }
            else
            {
                CurrentIndex = Math.Min(Items.Count - 1, _currentIndex + 1);
            }
        }

        /// <summary>
        /// 跳转到指定索引。
        /// </summary>
        public void GoTo(int index)
        {
            if (Items == null || Items.Count == 0 || index < 0 || index >= Items.Count)
                return;

            CurrentIndex = index;
        }

        private void UpdateIndicators()
        {
            if (_indicatorsPanel == null || Items == null)
                return;

            _indicatorsPanel.Children.Clear();

            for (int i = 0; i < Items.Count; i++)
            {
                var indicator = CreateIndicator(i);
                _indicatorsPanel.Children.Add(indicator);
            }
        }

        private FrameworkElement CreateIndicator(int index)
        {
            return IndicatorType switch
            {
                CarouselIndicatorType.Dot => CreateDotIndicator(index),
                CarouselIndicatorType.Number => CreateNumberIndicator(index),
                _ => CreateDotIndicator(index)
            };
        }

        private Border CreateDotIndicator(int index)
        {
            var dot = new Border
            {
                Width = 10,
                Height = 10,
                CornerRadius = new CornerRadius(5),
                Background = index == _currentIndex
                    ? new SolidColorBrush(Colors.White)
                    : new SolidColorBrush(Colors.White) { Opacity = 0.5 },
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = index,
                Margin = new Thickness(4, 0, 4, 0)
            };

            dot.MouseLeftButtonUp += (s, e) => GoTo(index);

            return dot;
        }

        private Button CreateNumberIndicator(int index)
        {
            var button = new Button
            {
                Content = (index + 1).ToString(),
                Margin = new Thickness(4, 0, 4, 0),
                Padding = new Thickness(8, 4, 8, 4),
                Tag = index
            };

            button.Click += (s, e) => GoTo(index);

            return button;
        }

        private void ApplyTransition()
        {
            if (_itemsControl == null || _itemsControl.ItemContainerGenerator == null)
                return;

            if (_currentIndex >= 0 && _currentIndex < Items.Count)
            {
                var container = _itemsControl.ItemContainerGenerator.ContainerFromIndex(_currentIndex) as UIElement;
                if (container != null)
                {
                    switch (Transition)
                    {
                        case CarouselTransition.Fade:
                            ApplyFadeTransition(container);
                            break;
                        case CarouselTransition.Scale:
                            ApplyScaleTransition(container);
                            break;
                        case CarouselTransition.Slide:
                            ApplySlideTransition(container);
                            break;
                    }
                }
            }

            RaiseEvent(new RoutedEventArgs(SelectionChangedEvent, this));
        }

        private void ApplyFadeTransition(UIElement element)
        {
            var storyboard = new Storyboard();

            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TransitionDuration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(fadeAnimation, element);
            Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath(OpacityProperty));

            storyboard.Children.Add(fadeAnimation);
            storyboard.Begin();
        }

        private void ApplyScaleTransition(UIElement element)
        {
            var storyboard = new Storyboard();

            var scaleXAnimation = new DoubleAnimation
            {
                From = 0.8,
                To = 1,
                Duration = TransitionDuration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            var scaleYAnimation = new DoubleAnimation
            {
                From = 0.8,
                To = 1,
                Duration = TransitionDuration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(scaleXAnimation, element);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));

            Storyboard.SetTarget(scaleYAnimation, element);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));

            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);
            storyboard.Begin();
        }

        private void ApplySlideTransition(UIElement element)
        {
            var storyboard = new Storyboard();

            var translateAnimation = new DoubleAnimation
            {
                From = 100,
                To = 0,
                Duration = TransitionDuration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(translateAnimation, element);
            Storyboard.SetTargetProperty(translateAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));

            storyboard.Children.Add(translateAnimation);
            storyboard.Begin();
        }

        #endregion
    }
}
