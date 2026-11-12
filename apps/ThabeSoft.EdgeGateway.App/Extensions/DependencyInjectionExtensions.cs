using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.App;
using ThabeSoft.EdgeGateway.App.Common;
using ThabeSoft.EdgeGateway.App.Features;
using ThabeSoft.EdgeGateway.App.Features.Dashboard;
using ThabeSoft.EdgeGateway.App.Features.Gateway;
using ThabeSoft.EdgeGateway.App.Features.Topology;
using ThabeSoft.EdgeGateway.App.Hosting;
using ThabeSoft.EdgeGateway.App.Services;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IHostBuilder builder)
    {
        public IHostBuilder UseEdgeGatewayApp()
        {
            builder.ConfigureLogging((_, builder) => builder
                .SetMinimumLevel(LogLevel.Information));

            builder.ConfigureServices(services =>
            {
                services.AddSimulation();
                services.AddModbusSimulation();
                //services.AddSimulationHost();

                services.AddEdgeGateway();
                //services.AddEdgeGatewayHost();
                services.AddSerial();
                services.AddEthernet();
                services.AddModbus();
                services.AddApp();
                services.AddAppHost();
            });

            return builder;
        }
    }


    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加视图模块
        /// </summary>
        public IServiceCollection AddApp()
        {
            // Services
            descriptors.AddSingleton<ClipboardService>();
            descriptors.AddSingleton<NavigationService>();

            descriptors.AddSingleton<ISukiToastManager, SukiToastManager>();
            descriptors.AddSingleton<ISukiDialogManager, SukiDialogManager>();


            // Views
            descriptors.AddSingleton<App.Common.ViewLocator>();
            descriptors.AddSingleton<ViewFactory>();


            // Gateway
            descriptors.AddSingleton<GatewayViewModel>();

            // Shell
            descriptors.AddViewMapping<MainWindow, MainWindowViewModel>(ServiceLifetime.Singleton);
            descriptors.AddSingleton<INavigationHost>(sp => sp.GetRequiredService<MainWindowViewModel>());

            // Pages
            descriptors.AddViewMapping<DashboardView, DashboardViewModel>(ServiceLifetime.Singleton);


            descriptors.AddViewMapping<TopologyView, TopologyViewModel>(ServiceLifetime.Singleton);
            descriptors.AddViewMapping<AddPortView, AddPortViewModel>(ServiceLifetime.Transient);

            return descriptors;
        }

        public IServiceCollection AddAppHost()
        {
            descriptors.AddHostedService<LaunchHostedService>();
            return descriptors;
        }


        public IServiceCollection AddViewMapping<TView, TViewModel>(ServiceLifetime lifetime)
            where TView : IDataContextProvider, new()
            where TViewModel : ObservableObject
        {
            var viewModelType = typeof(TViewModel);

            if (viewModelType.IsAssignableTo(typeof(PageViewModel)))
            {
                descriptors.Add(ServiceDescriptor.Describe(typeof(PageViewModel), viewModelType, lifetime));
            }
            else
            {
                descriptors.Add(ServiceDescriptor.Describe(viewModelType, viewModelType, lifetime));
            }

            return descriptors.AddSingleton(ViewMapping.Create<TView, TViewModel>());
        }
    }
}