using CommunityToolkit.Mvvm.DependencyInjection;
using Lemoo.UI.WPF.ViewModels;
using Lemoo.UI.WPF.ViewModels.Pages;
using System.Windows;

namespace Lemoo.UI.WPF.Views.Pages;

/// <summary>
/// 工具箱示例页面
/// </summary>
public partial class ToolboxSamplePage
{
    private readonly ToolboxSampleViewModel _viewModel;

    public ToolboxSamplePage()
    {
        InitializeComponent();

        // 创建 ViewModel
        _viewModel = Ioc.Default.GetRequiredService<ToolboxSampleViewModel>();
        DataContext = _viewModel;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // 在Loaded事件中连接 ToolboxView 的 ViewModel
        if (ToolboxViewControl.DataContext is ToolboxViewModel toolboxViewModel)
        {
            // 监听工具箱的选中控件变化
            toolboxViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ToolboxViewModel.SelectedControl))
                {
                    _viewModel.SelectedControl = toolboxViewModel.SelectedControl;
                }
            };
        }
    }
}
