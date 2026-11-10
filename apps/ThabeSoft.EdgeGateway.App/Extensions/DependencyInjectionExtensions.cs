using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System.Net;
using ThabeSoft.EdgeGateway;
using ThabeSoft.EdgeGateway.App;
using ThabeSoft.EdgeGateway.App.Common;
using ThabeSoft.EdgeGateway.App.Features.Dashboard;
using ThabeSoft.EdgeGateway.App.Features.Gateway;
using ThabeSoft.EdgeGateway.App.Features.Topology;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;

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
                services.AddEdgeGateway(x => x.DisabledAutoLauch());
                services.AddSerial();
                services.AddEthernet();
                services.AddModbus();
                services.AddApp();

#if DEBUG
                services.ConfigureGateway(gateway =>
                {
                    gateway.AddSerialPort("COM2")
                        .AddModbusRtuDevice()
                            .AddSubscription(Modbus.HoldingRegister(1, 0, Codec.Int16), Trigger.Interval(100))
                                .Subscribe(x =>
                                    Console.WriteLine(x)
                                );

                    //gateway.AddTcpPort(IPAddress.Loopback, 1)
                    //    .AddModbusIpDevice(DeviceId.From("Ip_Tcp_Device"))
                    //        .AddSubscription(Modbus.Coil(1, 0), Trigger.Interval(10));

                    //gateway.AddUdpPort(IPAddress.Loopback, 2)
                    //    .AddModbusIpDevice(DeviceId.From("Ip_Udp_Device"))
                    //        .AddSubscription(Modbus.Coil(1, 0), Trigger.Interval(10));
                });
#endif

                if (Design.IsDesignMode)
                {
                    // 模拟底层传输
                    services.RemoveAll<ITransportConnectionFactory>();
                    services.AddSingleton<ITransportConnectionFactory, MockTransportFactory>();
                }
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
    }
}


public sealed class MockTransportFactory(ILoggerFactory loggerFactory) : ITransportConnectionFactory
{
    public bool CanCreate(ITransportDescriptor descriptor) => true;
    public ITransportConnection Create(ITransportDescriptor descriptor) =>
        new MockTransportConnection(descriptor, loggerFactory.CreateLogger<MockTransportConnection>());


    private sealed class MockTransportConnection(ITransportDescriptor transportDescriptor, ILogger<MockTransportConnection> logger) : ITransportConnection
    {
        private IStrictLifecycleStateMachine _stateMachinee = Lifecycle.StrictStateMachine(logger);

        public ConnectionEndpoint Endpoint => transportDescriptor.Endpoint;
        public LifecycleState State => _stateMachinee.State;



        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _stateMachinee.ExecuteAsync(_ =>
            {
                logger.LogDebug("读取数据");
                return ValueTask.FromResult(0);
            }, cancellationToken);
        }
        public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            return _stateMachinee.ExecuteAsync(_ =>
            {
                logger.LogDebug("写入数据, 长度:{Len}", data.Length);
                return ValueTask.CompletedTask;
            }, cancellationToken);

        }


        public ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            return _stateMachinee.StartAsync(_ =>
            {
                logger.LogDebug("已启动");
                return ValueTask.CompletedTask;
            }, cancellationToken);
        }

        public ValueTask StopAsync(CancellationToken cancellationToken = default)
        {
            return _stateMachinee.StopAsync(_ =>
            {
                logger.LogDebug("已停止");
                return ValueTask.CompletedTask;
            }, cancellationToken);
        }
        public ValueTask DisposeAsync()
        {
            return _stateMachinee.DisposeAsync(() =>
            {
                logger.LogDebug("已释放");
                return ValueTask.CompletedTask;
            });
        }
    }
}

