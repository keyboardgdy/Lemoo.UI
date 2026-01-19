using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace Lemoo.UI.Models.CodeEditor
{
    /// <summary>
    /// JSON 语法高亮器。
    /// </summary>
    public class JsonSyntaxHighlighter : ISyntaxHighlighter
    {
        public CodeEditorLanguage Language => CodeEditorLanguage.JSON;

        private readonly CodeEditorColorScheme _colorScheme;

        public JsonSyntaxHighlighter()
        {
            _colorScheme = GetColorScheme();
        }

        public List<SyntaxHighlightResult> Highlight(string text)
        {
            var results = new List<SyntaxHighlightResult>();

            if (string.IsNullOrEmpty(text))
                return results;

            // 高亮字符串（键名和值）
            foreach (Match match in Regex.Matches(text, @"(""[^""]*"")\s*:"))
            {
                results.Add(new SyntaxHighlightResult
                {
                    StartIndex = match.Index,
                    Length = match.Length - 1, // 不包含冒号
                    Foreground = new SolidColorBrush(Color.FromRgb(156, 220, 254))
                });
            }

            // 高亮字符串值
            foreach (Match match in Regex.Matches(text, @":\s*(""[^""]*"")"))
            {
                var stringMatch = match.Groups[1];
                results.Add(new SyntaxHighlightResult
                {
                    StartIndex = stringMatch.Index,
                    Length = stringMatch.Length,
                    Foreground = _colorScheme.String
                });
            }

            // 高亮数字
            foreach (Match match in Regex.Matches(text, @":\s*(-?\d+\.?\d*(?:[eE][+-]?\d+)?)"))
            {
                var numberMatch = match.Groups[1];
                results.Add(new SyntaxHighlightResult
                {
                    StartIndex = numberMatch.Index,
                    Length = numberMatch.Length,
                    Foreground = _colorScheme.Number
                });
            }

            // 高亮布尔值和 null
            foreach (Match match in Regex.Matches(text, @"\b(true|false|null)\b"))
            {
                results.Add(new SyntaxHighlightResult
                {
                    StartIndex = match.Index,
                    Length = match.Length,
                    Foreground = new SolidColorBrush(Color.FromRgb(86, 156, 214))
                });
            }

            return results;
        }

        public CodeEditorColorScheme GetColorScheme()
        {
            return new CodeEditorColorScheme
            {
                Keyword = new SolidColorBrush(Color.FromRgb(86, 156, 214)),
                String = new SolidColorBrush(Color.FromRgb(206, 145, 120)),
                Comment = new SolidColorBrush(Color.FromRgb(106, 153, 85)),
                Number = new SolidColorBrush(Color.FromRgb(181, 206, 168)),
                Type = new SolidColorBrush(Color.FromRgb(78, 201, 176)),
                Function = new SolidColorBrush(Color.FromRgb(220, 220, 170)),
                Variable = Brushes.White,
                Operator = new SolidColorBrush(Color.FromRgb(212, 212, 212)),
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                LineNumber = new SolidColorBrush(Color.FromRgb(133, 133, 133)),
                CurrentLine = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                Selection = new SolidColorBrush(Color.FromRgb(38, 79, 120))
            };
        }
    }
}
