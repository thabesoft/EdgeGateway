using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System.Net;
using ThabeSoft.EdgeGateway.App;
using ThabeSoft.EdgeGateway.App.Common;
using ThabeSoft.EdgeGateway.App.Designs;
using ThabeSoft.EdgeGateway.App.Features.Dashboard;
using ThabeSoft.EdgeGateway.App.Features.Topology;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.App.ViewModels;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                .SetMinimumLevel(LogLevel.Trace));

            builder.ConfigureServices(services =>
            {
                services.AddApp();

#if DEBUG
                services.AddEdgeGateway(gateway =>
                {
                    gateway.AddSerialPort("COM2", configure: x => x
                        .AddModbusRtuDevice(DeviceId.From("Serial_Rtu_Device"))
                        .AddModbusAsciiDevice(DeviceId.From("Serial_Ascii_Device")));

                    gateway.AddTcpPort(IPAddress.Loopback, 1, x => x
                        .AddModbusIpDevice(DeviceId.From("Ip_Tcp_Device")));

                    gateway.AddUdpPort(IPAddress.Loopback, 2, x => x
                        .AddModbusIpDevice(DeviceId.From("Ip_Udp_Device")));
                });
#endif

                if (Design.IsDesignMode || true)
                {
                    // 模拟底层传输
                    services.RemoveAll<ITransportFactory>();
                    services.AddSingleton<ITransportFactory, MockTransportFactory>();
                    return;
                }



                services.AddSerial();
                services.AddEthernet();
                services.AddModbus();
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
            descriptors.AddSingleton<ViewLocator>();
            descriptors.AddSingleton<ViewFactory>();



            // Core
            descriptors.AddSingleton<GatewayViewModel>();

            descriptors.AddSingleton(sp =>
               new PortViewModelFactory(port =>
                   ActivatorUtilities.CreateInstance<PortViewModel>(sp, port)));
            descriptors.AddSingleton(sp =>
                new DeviceViewModelFactory(device =>
                    ActivatorUtilities.CreateInstance<DeviceViewModel>(sp, device)));

            // Shell
            descriptors.AddViewMapping<MainWindow, MainWindowViewModel>(ServiceLifetime.Singleton);
            descriptors.AddSingleton<INavigationHost>(sp => sp.GetRequiredService<MainWindowViewModel>());

            // Pages
            descriptors.AddViewMapping<DashboardView, DashboardViewModel>(ServiceLifetime.Singleton);


            descriptors.AddViewMapping<TopologyView, TopologyViewModel>(ServiceLifetime.Singleton);
            descriptors.AddViewMapping<AddPortView, AddPortViewModel>(ServiceLifetime.Transient);

            return descriptors;
        }
    }
}


public sealed class MockTransportFactory(ILoggerFactory loggerFactory) : ITransportFactory
{
    public bool CanCreate(ITransportDescriptor descriptor) => true;
    public ITransportConnection Create(ITransportDescriptor descriptor) =>
        new MockTransportConnection(descriptor, loggerFactory.CreateLogger<MockTransportConnection>());


    private sealed class MockTransportConnection(ITransportDescriptor transportDescriptor, ILogger<MockTransportConnection> logger) : LifecycleStateMachine, ITransportConnection
    {
        public ConnectionEndpoint Endpoint => transportDescriptor.Endpoint;

        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("读取数据");
            return ValueTask.FromResult(0);
        }
        public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("写入数据, 长度:{Len}", data.Length);
            return ValueTask.CompletedTask;
        }

        protected override ValueTask OnStartAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("已启动");
            return ValueTask.CompletedTask;
        }
        protected override ValueTask OnStopAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("已停止");
            return ValueTask.CompletedTask;
        }
    }
}

