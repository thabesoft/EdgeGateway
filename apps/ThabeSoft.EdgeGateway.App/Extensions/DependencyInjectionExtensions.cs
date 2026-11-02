using ThabeSoft.EdgeGateway.App;
using ThabeSoft.EdgeGateway.App.Dashboard;
using ThabeSoft.EdgeGateway.App.Dialog;
using ThabeSoft.EdgeGateway.App.Navigation;
using ThabeSoft.EdgeGateway.App.Port;
using ThabeSoft.EdgeGateway.App.Port.Device;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.App.Shell;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGatewayApp()
        {
            // Shell
            descriptors.AddSingleton<MainWindowViewModel>();
            descriptors.AddSingleton<ViewModelTmpleate>();

            descriptors.AddSingleton<MainViewModel>();
            descriptors.AddSingleton<INavigationService>(sp => sp.GetRequiredService<MainViewModel>());
            descriptors.AddSingleton<IDialogService>(sp => sp.GetRequiredService<MainViewModel>());
            descriptors.AddSingleton(ViewMapping.Create<MainViewModel, MainView>());


            // Dashboard
            descriptors.AddSingleton<DashboardViewModel>();


            // Port
            descriptors.AddSingleton<PortViewModel>();
            descriptors.AddSingleton(ViewMapping.Create<PortViewModel, PortView>());

            descriptors.AddTransient<AddPortViewModel>();
            descriptors.AddSingleton(ViewMapping.Create<AddPortViewModel, AddPortView>());

            descriptors.AddSingleton(sp =>
                new PortSummaryViewModelFactory(port =>
                    ActivatorUtilities.CreateInstance<PortSummaryViewModel>(sp, port)));
            descriptors.AddSingleton(sp =>
                new DeviceSummaryViewModelFactory(device =>
                    ActivatorUtilities.CreateInstance<DeviceSummaryViewModel>(sp, device)));

            // Device
            descriptors.AddTransient<DeviceViewModel>();
            descriptors.AddSingleton(ViewMapping.Create<DeviceViewModel, DeviceView>());

            // AliasServices
            descriptors.AddAliasService<DeviceId>();
            descriptors.AddAliasService<PortId>();


            return descriptors;
        }


        public IServiceCollection AddAliasService<TId>()
            where TId : IEquatable<TId>
        {
            return descriptors.AddSingleton<IAliasService<TId>, AliasService<TId>>();
        }
    }
}
