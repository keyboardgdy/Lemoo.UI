using System.Windows;

namespace Lemoo.UI.WPF.Services
{
    /// <summary>
    /// 剪贴板服务实现
    /// </summary>
    public class ClipboardService : IClipboardService
    {
        /// <inheritdoc/>
        public void SetText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Clipboard.SetText(text);
            }
        }

        /// <inheritdoc/>
        public string? GetText()
        {
            return Clipboard.ContainsText() ? Clipboard.GetText() : null;
        }
    }
}
