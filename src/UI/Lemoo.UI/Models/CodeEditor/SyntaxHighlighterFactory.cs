using System;
using System.Collections.Generic;

namespace Lemoo.UI.Models.CodeEditor
{
    /// <summary>
    /// 语法高亮器工厂。
    /// </summary>
    public static class SyntaxHighlighterFactory
    {
        private static readonly Dictionary<CodeEditorLanguage, ISyntaxHighlighter> Highlighters = new()
        {
            { CodeEditorLanguage.CSharp, new CSharpSyntaxHighlighter() },
            { CodeEditorLanguage.JSON, new JsonSyntaxHighlighter() },
            { CodeEditorLanguage.Text, new TextSyntaxHighlighter() }
        };

        /// <summary>
        /// 获取指定语言的语法高亮器。
        /// </summary>
        /// <param name="language">语言类型。</param>
        /// <returns>语法高亮器实例。</returns>
        public static ISyntaxHighlighter GetHighlighter(CodeEditorLanguage language)
        {
            if (Highlighters.TryGetValue(language, out var highlighter))
            {
                return highlighter;
            }

            return Highlighters[CodeEditorLanguage.Text];
        }

        /// <summary>
        /// 注册自定义语法高亮器。
        /// </summary>
        /// <param name="language">语言类型。</param>
        /// <param name="highlighter">语法高亮器实例。</param>
        public static void RegisterHighlighter(CodeEditorLanguage language, ISyntaxHighlighter highlighter)
        {
            Highlighters[language] = highlighter;
        }
    }
}
