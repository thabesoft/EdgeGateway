using EdgeGateway;
using EdgeGateway.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var config = GatewayConfig.Builder()
    .AddPort(port => port
        .Tcp("192.168.0.1", 8086)
        .AddModbusTcpDevice())
    .AddPort(port => port
        .Udp("192.168.1.12", 8086)
        .AddModbusAsciiDevice())
    .AddPort(p => p
        .Serial("COM1")
        .AddModbusRtuDevice())
    .Build();

await Host.CreateDefaultBuilder(args)
    .ConfigureServices(x => x
        .AddEdgeGateway(config)
        .AddSerial()
        .AddEthernet()
        .AddModbus()
        .AddHostedService<GatewayHostedService>()
        .AddHostedService<LaunchedService>()
        )
    .RunConsoleAsync();