using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lemoo.UI.Models.Icons;
using Lemoo.UI.WPF.Services;

namespace Lemoo.UI.WPF.ViewModels.Pages
{
    /// <summary>
    /// 图标网格项视图模型
    /// </summary>
    public partial class IconItemViewModel : ObservableObject
    {
        private readonly IClipboardService _clipboardService;
        private readonly INotificationService _notificationService;
        private bool _isSelected;

        /// <summary>
        /// 图标信息
        /// </summary>
        public IconInfo IconInfo { get; }

        /// <summary>
        /// 图标类型
        /// </summary>
        public IconKind Kind => IconInfo.Kind;

        /// <summary>
        /// 图标名称
        /// </summary>
        public string Name => IconInfo.Name;

        /// <summary>
        /// 图标字形
        /// </summary>
        public string Glyph => IconInfo.Glyph;

        /// <summary>
        /// 图标分类
        /// </summary>
        public string Category => IconInfo.Category;

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public IconItemViewModel(
            IconInfo iconInfo,
            IClipboardService clipboardService,
            INotificationService notificationService)
        {
            IconInfo = iconInfo;
            _clipboardService = clipboardService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// 复制图标代码命令
        /// </summary>
        [RelayCommand]
        private void CopyIconCode()
        {
            var code = $"IconKind.{Kind}";
            _clipboardService.SetText(code);
            _notificationService.ShowSuccess($"已复制: {code}");
        }

        /// <summary>
        /// 复制字形命令
        /// </summary>
        [RelayCommand]
        private void CopyGlyph()
        {
            _clipboardService.SetText(Glyph);
            _notificationService.ShowSuccess($"已复制字形: {Name}");
        }

        /// <summary>
        /// 复制Unicode命令
        /// </summary>
        [RelayCommand]
        private void CopyUnicode()
        {
            if (Glyph.Length > 0)
            {
                int codePoint = char.ConvertToUtf32(Glyph, 0);
                var unicode = $"U+{codePoint:X4}";
                _clipboardService.SetText(unicode);
                _notificationService.ShowSuccess($"已复制Unicode: {unicode}");
            }
        }
    }
}
