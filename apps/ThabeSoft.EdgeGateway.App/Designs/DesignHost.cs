using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Options;

namespace ThabeSoft.EdgeGateway.App.Designs;


internal static class DesignHost
{
    private readonly static IServiceProvider? _services;

#if DEBUG
    private readonly static IHost? _host;

    static DesignHost()
    {
        if (!Design.IsDesignMode) return;

        var builder = Host.CreateDefaultBuilder()
            .ConfigureLogging((_, builder) =>
                builder.ClearProviders().AddDebug().SetMinimumLevel(LogLevel.Trace))
            .ConfigureServices(services =>
            {
                services.AddEdgeGateway(gateway =>
                {
                    gateway.AddSerialPort("COM2", portBuildAction: x => x
                        .AddModbusRtuDevice(DeviceId.From("Serial_Rtu_Device"))
                        .AddModbusAsciiDevice(DeviceId.From("Serial_Ascii_Device")));

                    gateway.AddTcpPort(IPAddress.Loopback, 1, x => x
                        .AddModbusIpDevice(DeviceId.From("Ip_Tcp_Device")));

                    gateway.AddUdpPort(IPAddress.Loopback, 2, x => x
                        .AddModbusIpDevice(DeviceId.From("Ip_Udp_Device")));
                });
                services.AddSingleton<IGateway, DesignGateway>();

                services.AddApp();
            });

        _host = builder.Build();
        _services = _host.Services;
        _ = _host.StartAsync();
    }

#endif


    public static T? GetService<T>()
        where T : notnull
    {
        if (_services is null) return default;
        if (!Design.IsDesignMode) return default;

        return _services.GetService<T>();
    }

    public static bool SetDataContext<T>(IDataContextProvider dataContextProvider)
         where T : notnull
    {
        var dataContext = GetService<T>();
        if (dataContext is null) return false;

        dataContextProvider.DataContext = dataContext;
        return true;
    }
}