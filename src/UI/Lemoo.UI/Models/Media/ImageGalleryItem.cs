using System;
using System.Windows;

namespace Lemoo.UI.Models.Media
{
    /// <summary>
    /// 图片画廊项数据模型。
    /// </summary>
    public class ImageGalleryItem : DependencyObject
    {
        #region ImageSource 依赖属性

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                nameof(ImageSource),
                typeof(string),
                typeof(ImageGalleryItem),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置图片源路径。
        /// </summary>
        public string ImageSource
        {
            get => (string)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        #endregion

        #region ThumbnailSource 依赖属性

        public static readonly DependencyProperty ThumbnailSourceProperty =
            DependencyProperty.Register(
                nameof(ThumbnailSource),
                typeof(string),
                typeof(ImageGalleryItem),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置缩略图源路径。
        /// </summary>
        public string ThumbnailSource
        {
            get => (string)GetValue(ThumbnailSourceProperty);
            set => SetValue(ThumbnailSourceProperty, value);
        }

        #endregion

        #region Title 依赖属性

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(ImageGalleryItem),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置图片标题。
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

        #region Description 依赖属性

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(
                nameof(Description),
                typeof(string),
                typeof(ImageGalleryItem),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取或设置图片描述。
        /// </summary>
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        #endregion

        #region Tag 依赖属性

        public static readonly DependencyProperty TagProperty =
            DependencyProperty.Register(
                nameof(Tag),
                typeof(object),
                typeof(ImageGalleryItem),
                new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置关联的自定义数据。
        /// </summary>
        public object? Tag
        {
            get => GetValue(TagProperty);
            set => SetValue(TagProperty, value);
        }

        #endregion

        #region IsSelected 依赖属性

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                nameof(IsSelected),
                typeof(bool),
                typeof(ImageGalleryItem),
                new PropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否选中。
        /// </summary>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        #endregion

        #region DateTime 依赖属性

        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register(
                nameof(DateTime),
                typeof(DateTime),
                typeof(ImageGalleryItem),
                new PropertyMetadata(DateTime.MinValue));

        /// <summary>
        /// 获取或设置图片创建时间。
        /// </summary>
        public DateTime DateTime
        {
            get => (DateTime)GetValue(DateTimeProperty);
            set => SetValue(DateTimeProperty, value);
        }

        #endregion
    }
}
