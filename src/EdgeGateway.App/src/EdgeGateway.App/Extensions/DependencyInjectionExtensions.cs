using EdgeGateway.App.DataTemplates;
using EdgeGateway.App.Models;
using EdgeGateway.App.Servies;
using EdgeGateway.App.ViewModels;
using EdgeGateway.App.Views;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGatewayApp()
        {
            // 视图模型
            descriptors.AddSingleton<MainWindowViewModel>();
            descriptors.AddSingleton<MainViewModel>();
            descriptors.AddSingleton<INavigationService>(sp => sp.GetRequiredService<MainViewModel>());
            descriptors.AddSingleton<IDialogService>(sp => sp.GetRequiredService<MainViewModel>());

            descriptors.AddSingleton<PortViewModel>();
            descriptors.AddSingleton<DashboardViewModel>();
            descriptors.AddTransient<AddPortViewModel>();

            // 视图模型与视图映射
            descriptors.AddSingleton(ViewMapping.Create<MainViewModel, MainView>());
            descriptors.AddSingleton(ViewMapping.Create<PortViewModel, PortView>());
            descriptors.AddSingleton(ViewMapping.Create<AddPortViewModel, AddPortView>());

            // 数据模板
            descriptors.AddSingleton<ViewModelTmpleate>();

            return descriptors;
        }
    }
}
