using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.App.DataTmpleates;
using ThabeSoft.EdgeGateway.App.Models;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.App.ViewModels;
using ThabeSoft.EdgeGateway.App.Views;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Options;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IHostBuilder builder)
    {
        public IHostBuilder UseEdgeGatewayApp(Action<GatewayOptionsBuilder>? buildAction = null)
        {
            builder.ConfigureLogging((_, builder) => builder
                .SetMinimumLevel(LogLevel.Trace));

            builder.ConfigureServices(services => services
                .AddEdgeGateway(gateway =>
                {
#if DEBUG
                    gateway.AddSerialPort("COM2", portBuildAction: x => x.AddModbusIpDevice());
#endif
                    buildAction?.Invoke(gateway);
                })
                .AddSerial()
                .AddEthernet()
                .AddModbus()
                .AddApp());

            return builder;
        }
    }


    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加视图模块
        /// </summary>
        /// <returns></returns>
        public IServiceCollection AddApp()
        {
            // DataTemplates
            descriptors.AddSingleton<ViewModelDataTmpleate>();

            // AliasServices
            descriptors.AddAliasService<DeviceId>();

            // Services
            descriptors.AddSingleton<INavigationService>(sp => sp.GetRequiredService<MainViewModel>());
            descriptors.AddSingleton<IDialogService>(sp => sp.GetRequiredService<MainViewModel>());

            // Core
            descriptors.AddSingleton<GatewayViewModel>();

            descriptors.AddSingleton(sp =>
               new PortViewModelFactory(port =>
                   ActivatorUtilities.CreateInstance<PortViewModel>(sp, port)));
            descriptors.AddSingleton(sp =>
                new DeviceViewModelFactory(device =>
                    ActivatorUtilities.CreateInstance<DeviceViewModel>(sp, device)));

            // Shell
            descriptors.AddSingleton<MainWindowViewModel>();
            descriptors.AddSingleton<MainViewModel>();

            // Dashboard
            descriptors.AddSingleton<DashboardPageViewModel>();

            // Manage
            descriptors.AddMvvmPair<ManageViewModel, ManageView>(ServiceLifetime.Singleton);
            descriptors.AddMvvmPair<AddPortViewModel, AddPortView>(ServiceLifetime.Transient);

            return descriptors;
        }


        public IServiceCollection AddAliasService<TId>()
            where TId : IEquatable<TId>
        {
            return descriptors.AddSingleton<IAliasService<TId>, AliasService<TId>>();
        }

        private void AddMvvmPair<TViewModel, TView>(ServiceLifetime serviceLifetime)
            where TViewModel : ViewModelBase
            where TView : IDataContextProvider, new()
        {
            descriptors.Add(ServiceDescriptor.Describe(typeof(TViewModel), typeof(TViewModel), serviceLifetime));
            descriptors.AddSingleton(ViewMapping.Create<TViewModel, TView>());
        }
    }
}
