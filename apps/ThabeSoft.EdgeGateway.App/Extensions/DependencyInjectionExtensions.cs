using Avalonia;
using ThabeSoft.EdgeGateway.App.DataTmpleates;
using ThabeSoft.EdgeGateway.App.Models;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.App.ViewModels;
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
            // Shell
            descriptors.AddSingleton<MainWindowViewModel>();
            descriptors.AddSingleton<MainViewModel>();


            // Dashboard
            descriptors.AddSingleton<DashboardViewModel>();


            // Port
            descriptors.AddSingleton<PortViewModel>();
            descriptors.AddViewMapping<PortViewModel, PortView>();

            descriptors.AddTransient<AddPortViewModel>();
            descriptors.AddViewMapping<AddPortViewModel, AddPortView>();

            descriptors.AddSingleton(sp =>
               new PortSummaryViewModelFactory(port =>
                   ActivatorUtilities.CreateInstance<PortSummaryViewModel>(sp, port)));
            descriptors.AddSingleton(sp =>
                new DeviceSummaryViewModelFactory(device =>
                    ActivatorUtilities.CreateInstance<DeviceSummaryViewModel>(sp, device)));


            descriptors.AddTransient<DeviceViewModel>();
            descriptors.AddSingleton(ViewMapping.Create<DeviceViewModel, DeviceView>());
        }
    }
}
