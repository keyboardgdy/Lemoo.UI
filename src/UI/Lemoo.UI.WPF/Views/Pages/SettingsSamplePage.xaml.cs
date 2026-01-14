using Lemoo.UI.WPF.ViewModels.Pages;

namespace Lemoo.UI.WPF.Views.Pages;

/// <summary>
/// 设置示例页面
/// </summary>
public partial class SettingsSamplePage
{
    public SettingsSamplePage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
