using Lemoo.UI.ViewModels;

namespace Lemoo.UI.Controls.Toolbox;

/// <summary>
/// ToolboxView.xaml 的交互逻辑
/// </summary>
public partial class ToolboxView : System.Windows.Controls.UserControl
{
    public ToolboxView()
    {
        InitializeComponent();
        this.DataContext = new ToolboxViewModel();
    }

    /// <summary>
    /// 获取当前选中的控件XAML代码
    /// </summary>
    public string GetSelectedXaml()
    {
        if (DataContext is ToolboxViewModel viewModel)
        {
            return viewModel.GetInsertXaml();
        }
        return string.Empty;
    }
}
