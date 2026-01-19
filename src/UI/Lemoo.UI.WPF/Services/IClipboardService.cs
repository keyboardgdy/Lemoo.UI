namespace Lemoo.UI.WPF.Services
{
    /// <summary>
    /// 剪贴板服务接口
    /// </summary>
    public interface IClipboardService
    {
        /// <summary>
        /// 设置剪贴板文本
        /// </summary>
        void SetText(string text);

        /// <summary>
        /// 获取剪贴板文本
        /// </summary>
        string? GetText();
    }
}
