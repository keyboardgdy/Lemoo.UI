using System.Windows;
using System.Windows.Media;

namespace Lemoo.UI.Models.CodeEditor
{
    /// <summary>
    /// 语法高亮结果。
    /// </summary>
    public class SyntaxHighlightResult
    {
        /// <summary>
        /// 获取或设置高亮起始位置。
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// 获取或设置高亮长度。
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 获取或设置前景色。
        /// </summary>
        public Brush? Foreground { get; set; }

        /// <summary>
        /// 获取或设置背景色。
        /// </summary>
        public Brush? Background { get; set; }

        /// <summary>
        /// 获取或设置字体粗细。
        /// </summary>
        public FontWeight? Weight { get; set; }

        /// <summary>
        /// 获取或设置字体样式。
        /// </summary>
        public FontStyle? Style { get; set; }
    }
}
