using CommunityToolkit.Mvvm.ComponentModel;

namespace Lemoo.UI.WPF.ViewModels.Pages;

/// <summary>
/// 仪表盘页面视图模型
/// </summary>
public partial class DashboardViewModel : ObservableObject
{
    /// <summary>
    /// 欢迎消息
    /// </summary>
    [ObservableProperty]
    private string welcomeMessage = "欢迎使用 Lemoo.UI";

    /// <summary>
    /// 默认构造函数
    /// </summary>
    public DashboardViewModel()
    {
    }
}
