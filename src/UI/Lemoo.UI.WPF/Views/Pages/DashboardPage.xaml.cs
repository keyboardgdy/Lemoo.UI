using System.Windows.Controls;
using Lemoo.UI.WPF.ViewModels.Pages;

namespace Lemoo.UI.WPF.Views.Pages;

/// <summary>
/// 一个简单的仪表盘示例页面，用于测试导航与标签页承载效果。
/// </summary>
public partial class DashboardPage : Page
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
