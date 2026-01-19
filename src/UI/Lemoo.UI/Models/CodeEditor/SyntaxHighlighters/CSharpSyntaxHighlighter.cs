using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace Lemoo.UI.Models.CodeEditor
{
    /// <summary>
    /// C# 语法高亮器。
    /// </summary>
    public class CSharpSyntaxHighlighter : ISyntaxHighlighter
    {
        public CodeEditorLanguage Language => CodeEditorLanguage.CSharp;

        // C# 关键字
        private static readonly string[] Keywords = new[]
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
            "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
            "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
            "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true",
            "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual",
            "void", "volatile", "while", "await", "async", "var", "value", "when", "where", "yield"
        };

        private readonly CodeEditorColorScheme _colorScheme;

        public CSharpSyntaxHighlighter()
        {
            _colorScheme = GetColorScheme();
        }

        public List<SyntaxHighlightResult> Highlight(string text)
        {
            var results = new List<SyntaxHighlightResult>();

            if (string.IsNullOrEmpty(text))
                return results;

            // 移除字符串和注释，避免内部内容被高亮
            var processedText = text;
            var excludedRanges = new List<Tuple<int, int>>();

            // 高亮字符串
            foreach (Match match in Regex.Matches(processedText, @"(@?)(?>("")|([^""]))"))
            {
                if (match.Success)
                {
                    results.Add(new SyntaxHighlightResult
                    {
                        StartIndex = match.Index,
                        Length = match.Length,
                        Foreground = _colorScheme.String
                    });
                    excludedRanges.Add(Tuple.Create(match.Index, match.Length));
                }
            }

            // 高亮单行注释
            foreach (Match match in Regex.Matches(processedText, @"//.*$", RegexOptions.Multiline))
            {
                if (!IsInExcludedRange(match.Index, excludedRanges))
                {
                    results.Add(new SyntaxHighlightResult
                    {
                        StartIndex = match.Index,
                        Length = match.Length,
                        Foreground = _colorScheme.Comment
                    });
                    excludedRanges.Add(Tuple.Create(match.Index, match.Length));
                }
            }

            // 高亮多行注释
            foreach (Match match in Regex.Matches(processedText, @"/\*.*?\*/", RegexOptions.Singleline))
            {
                if (!IsInExcludedRange(match.Index, excludedRanges))
                {
                    results.Add(new SyntaxHighlightResult
                    {
                        StartIndex = match.Index,
                        Length = match.Length,
                        Foreground = _colorScheme.Comment
                    });
                    excludedRanges.Add(Tuple.Create(match.Index, match.Length));
                }
            }

            // 高亮关键字
            foreach (var keyword in Keywords)
            {
                var pattern = $@"\b{Regex.Escape(keyword)}\b";
                foreach (Match match in Regex.Matches(processedText, pattern))
                {
                    if (!IsInExcludedRange(match.Index, excludedRanges))
                    {
                        results.Add(new SyntaxHighlightResult
                        {
                            StartIndex = match.Index,
                            Length = match.Length,
                            Foreground = _colorScheme.Keyword
                        });
                    }
                }
            }

            // 高亮数字
            foreach (Match match in Regex.Matches(processedText, @"\b\d+\.?\d*\b"))
            {
                if (!IsInExcludedRange(match.Index, excludedRanges))
                {
                    results.Add(new SyntaxHighlightResult
                    {
                        StartIndex = match.Index,
                        Length = match.Length,
                        Foreground = _colorScheme.Number
                    });
                }
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

        private bool IsInExcludedRange(int position, List<Tuple<int, int>> ranges)
        {
            foreach (var (start, length) in ranges)
            {
                if (position >= start && position < start + length)
                    return true;
            }
            return false;
        }
    }
}
