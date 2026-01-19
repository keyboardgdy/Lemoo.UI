using System.Collections.Generic;
using System.Windows.Media;

namespace Lemoo.UI.Models.CodeEditor
{
    /// <summary>
    /// 纯文本语法高亮器（无高亮）。
    /// </summary>
    public class TextSyntaxHighlighter : ISyntaxHighlighter
    {
        public CodeEditorLanguage Language => CodeEditorLanguage.Text;

        public List<SyntaxHighlightResult> Highlight(string text)
        {
            return new List<SyntaxHighlightResult>();
        }

        public CodeEditorColorScheme GetColorScheme()
        {
            return new CodeEditorColorScheme
            {
                Keyword = Brushes.White,
                String = Brushes.White,
                Comment = new SolidColorBrush(Color.FromRgb(106, 153, 85)),
                Number = Brushes.White,
                Type = Brushes.White,
                Function = Brushes.White,
                Variable = Brushes.White,
                Operator = Brushes.White,
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                LineNumber = new SolidColorBrush(Color.FromRgb(133, 133, 133)),
                CurrentLine = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                Selection = new SolidColorBrush(Color.FromRgb(38, 79, 120))
            };
        }
    }
}
