using System.Windows;
using System.Windows.Controls;
using Lemoo.UI.WPF.Services;
using Lemoo.UI.WPF.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.UI.WPF.Views.Pages
{
    /// <summary>
    /// IconBrowserPageRefactored.xaml 的交互逻辑
    /// 重构版：符合WPF MVVM最佳实践
    /// </summary>
    public partial class IconBrowserPageRefactored : Page
    {
        /// <summary>
        /// 视图模型
        /// </summary>
        public IconBrowserPageViewModelRefactored ViewModel { get; }

        /// <summary>
        /// 构造函数（依赖注入）
        /// </summary>
        public IconBrowserPageRefactored()
        {
            InitializeComponent();

            // 从服务容器获取服务
            var app = (App)Application.Current;
            var serviceProvider = app.Services ?? throw new InvalidOperationException("Service provider not initialized");
            var clipboardService = serviceProvider.GetRequiredService<IClipboardService>();
            var notificationService = serviceProvider.GetRequiredService<INotificationService>();

            // 创建ViewModel
            ViewModel = new IconBrowserPageViewModelRefactored(clipboardService, notificationService);
            DataContext = ViewModel;
        }

        /// <summary>
        /// 构造函数（支持直接传入ViewModel，方便测试）
        /// </summary>
        public IconBrowserPageRefactored(IconBrowserPageViewModelRefactored viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;
        }
    }
}
