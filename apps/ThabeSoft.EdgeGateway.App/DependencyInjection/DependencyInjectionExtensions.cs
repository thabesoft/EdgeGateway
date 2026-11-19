using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.App.Common;
using ThabeSoft.EdgeGateway.App.Features;
using ThabeSoft.EdgeGateway.App.Features.Dashboard;
using ThabeSoft.EdgeGateway.App.Features.Gateway;
using ThabeSoft.EdgeGateway.App.Features.Topology;
using ThabeSoft.EdgeGateway.App.Hosting;
using ThabeSoft.EdgeGateway.App.Services;

namespace ThabeSoft.EdgeGateway.App.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加视图模块
        /// </summary>
        public IServiceCollection AddEdgeGatewayApp()
        {
            // Services
            
            descriptors.AddSingleton<NavigationService>();

            descriptors.AddSingleton<ISukiToastManager, SukiToastManager>();
            descriptors.AddSingleton<ISukiDialogManager, SukiDialogManager>();


            // Views
            descriptors.AddSingleton<Common.ViewLocator>();
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

        public IServiceCollection AddEdgeGatewayAppHost()
        {
            descriptors.AddHostedService<LaunchHostedService>();
            return descriptors;
        }




        private IServiceCollection AddViewMapping<TView, TViewModel>(ServiceLifetime lifetime)
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