using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ThabeSoft.Codecs.DependencyInjection;
using ThabeSoft.Configuration.DependencyInjection;
using ThabeSoft.EdgeGateway.App.DependencyInjection;
using ThabeSoft.EdgeGateway.App.Desktop.DependencyInjection;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.DependencyInjection;
using ThabeSoft.Lifecycles.DependencyInjection;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.Protocols.DependencyInjection;
using ThabeSoft.Protocols.Modbus.DependencyInjection;
using ThabeSoft.PubSub.DependencyInjection;
using ThabeSoft.Transports.DependencyInjection;
using ThabeSoft.Transports.Ethernet.DependencyInjection;
using ThabeSoft.Transports.Serial.DependencyInjection;
using ThabeSoft.Triggers.DependencyInjection;

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
            .ConfigureServices((context, descriptors) =>
            {
                /*---- 事件总线 ----*/
                descriptors.AddPubSub();


                /*---- 生命周期 ----*/
                descriptors.AddLifecycles();


                /*---- 触发器 ----*/
                descriptors.AddTriggers();
                descriptors.AddTriggersConfig();


                /*---- 编码器 ----*/
                descriptors.AddCodecs();
                descriptors.AddCodecsConfig();


                /*---- 传输 ----*/
                descriptors.AddTransports();
                // 串行
                descriptors.AddSerialTransport();
                descriptors.AddSerialTransportConfig();
                // 以太
                descriptors.AddEthernetTransport();
                descriptors.AddEthernetTransportConfig();


                /*---- 协议 ----*/
                descriptors.AddProtocols();
                // Modbus
                descriptors.AddModbusProtocol();
                descriptors.AddModbusProtocolConfig();


                /*---- 网关 ----*/
                descriptors.AddEdgeGateway();
                descriptors.AddEdgeGatewayHost();
                descriptors.AddEdgeGatewayConfig();
                // 配置
                var configFilePath = context.Configuration.GetValue<string>("Gateway:Config:FilePath").Required("缺少配置文件路径");
                var configRealoadDebounce = context.Configuration.GetValue<TimeSpan>("Gateway:Config:Debounce");
                descriptors.AddJsonConfig<GatewayConfig>(configFilePath, configRealoadDebounce);


                /*---- 客户端 ----*/
                descriptors.AddEdgeGatewayApp();
                descriptors.AddEdgeGatewayAppDesktop();
            });
    }
}
