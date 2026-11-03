using Avalonia;
using ThabeSoft.EdgeGateway.App.DataTmpleates;
using ThabeSoft.EdgeGateway.App.Models;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.App.ViewModels;
using ThabeSoft.EdgeGateway.App.ViewModels.Dashboard;
using ThabeSoft.EdgeGateway.App.Views;
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
            // DataTemplates
            descriptors.AddSingleton<ViewModelDataTmpleate>();

            // AliasServices
            descriptors.AddAliasService<DeviceId>();
            descriptors.AddAliasService<PortId>();

            // Services
            descriptors.AddSingleton<INavigationService>(sp => sp.GetRequiredService<MainViewModel>());
            descriptors.AddSingleton<IDialogService>(sp => sp.GetRequiredService<MainViewModel>());


            descriptors.AddViewModels();

            return descriptors;
        }


        public IServiceCollection AddAliasService<TId>()
            where TId : IEquatable<TId>
        {
            return descriptors.AddSingleton<IAliasService<TId>, AliasService<TId>>();
        }
        public IServiceCollection AddViewMapping<TViewModel, TView>()
            where TViewModel : ViewModelBase
            where TView : IDataContextProvider, new()
        {
            return descriptors.AddSingleton(ViewMapping.Create<TViewModel, TView>());
        }


        private void AddViewModels()
        {
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
