using System.Windows.Controls;

namespace Lemoo.UI.Models.Media
{
    /// <summary>
    /// 图片画廊显示模式。
    /// </summary>
    public enum ImageGalleryMode
    {
        /// <summary>
        /// 网格视图
        /// </summary>
        Grid,

        /// <summary>
        /// 列表视图
        /// </summary>
        List,

        /// <summary>
        /// 瀑布流视图
        /// </summary>
        Masonry
    }

    /// <summary>
    /// 图片缩放模式。
    /// </summary>
    public enum ImageZoomMode
    {
        /// <summary>
        /// 适应窗口
        /// </summary>
        Fit,

        /// <summary>
        /// 填充窗口
        /// </summary>
        Fill,

        /// <summary>
        /// 实际大小
        /// </summary>
        Actual,

        /// <summary>
        /// 自定义缩放
        /// </summary>
        Custom
    }
}
