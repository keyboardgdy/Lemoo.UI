using CommunityToolkit.Mvvm.ComponentModel;

namespace Lemoo.UI.WPF.ViewModels.Pages
{
    /// <summary>
    /// 图标分类项视图模型
    /// </summary>
    public partial class IconCategoryItemViewModel : ObservableObject
    {
        private bool _isSelected;

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// 图标数量
        /// </summary>
        public int IconCount { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                SetProperty(ref _isSelected, value);
            }
        }

        /// <summary>
        /// 显示文本（名称 + 数量）
        /// </summary>
        public string DisplayText => $"{DisplayName} ({IconCount})";
    }
}
