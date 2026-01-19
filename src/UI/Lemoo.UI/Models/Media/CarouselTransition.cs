namespace Lemoo.UI.Models.Media
{
    /// <summary>
    /// 轮播过渡效果类型。
    /// </summary>
    public enum CarouselTransition
    {
        /// <summary>
        /// 无效果
        /// </summary>
        None,

        /// <summary>
        /// 滑动效果
        /// </summary>
        Slide,

        /// <summary>
        /// 淡入淡出效果
        /// </summary>
        Fade,

        /// <summary>
        /// 缩放效果
        /// </summary>
        Scale,

        /// <summary>
        /// 立方体旋转效果
        /// </summary>
        Cube
    }

    /// <summary>
    /// 轮播指示器类型。
    /// </summary>
    public enum CarouselIndicatorType
    {
        /// <summary>
        /// 点状指示器
        /// </summary>
        Dot,

        /// <summary>
        /// 数字指示器
        /// </summary>
        Number,

        /// <summary>
        /// 缩略图指示器
        /// </summary>
        Thumbnail
    }
}
