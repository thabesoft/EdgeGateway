using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.App.Hosting;
using ThabeSoft.Codecs.DependencyInjection;
using ThabeSoft.Transports.Ethernet.DependencyInjection;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.Lifecycles.DependencyInjection;
using ThabeSoft.Protocols.Modbus.DependencyInjection;
using ThabeSoft.Transports.Serial.DependencyInjection;
using ThabeSoft.Triggers.DependencyInjection;
using ThabeSoft.Transports.DependencyInjection;
using ThabeSoft.Protocols.DependencyInjection;
using ThabeSoft.Configuration.DependencyInjection;
using ThabeSoft.EdgeGateway.Configuration;

namespace ThabeSoft.EdgeGateway.App.Desktop;


internal static class EdgeGatewayHost
{
    public static IHost Create()
    {
        return Host.CreateDefaultBuilder()
            .UseEdgeGatewayDesktopApp()
            .Build();
    }

    public static IHostBuilder UseEdgeGatewayDesktopApp(this IHostBuilder hostBuilder)
    {
        return hostBuilder
            .UseEdgeGateway()
            //.UseJsonConfig("Gateway:Config")
            .UseEdgeGatewayApp()
            .ConfigureServices(descriptors =>
            {
                descriptors.AddConfigPaths();
                descriptors.AddConfigMonitor<GatewayConfig>();
                descriptors.AddJsonConfigNodeParse();


                // 生命周期
                descriptors.AddLifecycles();

                // 触发器
                descriptors.AddTriggers();
                descriptors.AddTriggersConfig();

                // 编码器
                descriptors.AddCodecs();
                descriptors.AddCodecsConfig();

                // 传输
                descriptors.AddTransports();

                descriptors.AddSerialTransport();
                descriptors.AddSerialTransportConfig();

                descriptors.AddEthernetTransport();
                descriptors.AddEthernetTransportConfig();

                // 协议
                descriptors.AddProtocols();

                descriptors.AddModbusProtocol();
                descriptors.AddModbusProtocolConfig();
            });
    }
}
