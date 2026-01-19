using System.Collections.Generic;
using System.Windows.Media;

namespace Lemoo.UI.Models.CodeEditor
{
    /// <summary>
    /// 语法高亮器接口。
    /// </summary>
    public interface ISyntaxHighlighter
    {
        /// <summary>
        /// 获取语言类型。
        /// </summary>
        CodeEditorLanguage Language { get; }

        /// <summary>
        /// 对文本进行语法高亮。
        /// </summary>
        /// <param name="text">要高亮的文本。</param>
        /// <returns>语法高亮结果列表。</returns>
        List<SyntaxHighlightResult> Highlight(string text);

        /// <summary>
        /// 获取语言的默认颜色方案。
        /// </summary>
        CodeEditorColorScheme GetColorScheme();
    }

    /// <summary>
    /// 代码编辑器颜色方案。
    /// </summary>
    public class CodeEditorColorScheme
    {
        /// <summary>
        /// 关键字颜色。
        /// </summary>
        public Brush Keyword { get; set; } = new SolidColorBrush(Color.FromRgb(86, 156, 214));

        /// <summary>
        /// 字符串颜色。
        /// </summary>
        public Brush String { get; set; } = new SolidColorBrush(Color.FromRgb(206, 145, 120));

        /// <summary>
        /// 注释颜色。
        /// </summary>
        public Brush Comment { get; set; } = new SolidColorBrush(Color.FromRgb(106, 153, 85));

        /// <summary>
        /// 数字颜色。
        /// </summary>
        public Brush Number { get; set; } = new SolidColorBrush(Color.FromRgb(181, 206, 168));

        /// <summary>
        /// 类型颜色。
        /// </summary>
        public Brush Type { get; set; } = new SolidColorBrush(Color.FromRgb(78, 201, 176));

        /// <summary>
        /// 函数颜色。
        /// </summary>
        public Brush Function { get; set; } = new SolidColorBrush(Color.FromRgb(220, 220, 170));

        /// <summary>
        /// 变量颜色。
        /// </summary>
        public Brush Variable { get; set; } = Brushes.White;

        /// <summary>
        /// 运算符颜色。
        /// </summary>
        public Brush Operator { get; set; } = new SolidColorBrush(Color.FromRgb(212, 212, 212));

        /// <summary>
        /// 背景颜色。
        /// </summary>
        public Brush Background { get; set; } = new SolidColorBrush(Color.FromRgb(30, 30, 30));

        /// <summary>
        /// 前景颜色。
        /// </summary>
        public Brush Foreground { get; set; } = new SolidColorBrush(Color.FromRgb(220, 220, 220));

        /// <summary>
        /// 行号颜色。
        /// </summary>
        public Brush LineNumber { get; set; } = new SolidColorBrush(Color.FromRgb(133, 133, 133));

        /// <summary>
        /// 当前行高亮颜色。
        /// </summary>
        public Brush CurrentLine { get; set; } = new SolidColorBrush(Color.FromRgb(40, 40, 40));

        /// <summary>
        /// 选中文本背景色。
        /// </summary>
        public Brush Selection { get; set; } = new SolidColorBrush(Color.FromRgb(38, 79, 120));
    }
}
