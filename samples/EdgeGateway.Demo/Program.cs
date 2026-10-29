using EdgeGateway;
using EdgeGateway.Configs;
using Microsoft.Extensions.Hosting;



var builder = GatewayConfig.Builder();

builder.AddPort(port => port
    .Tcp("192.168.0.1", 8086)
    .AddModbusIpDevice());

builder.AddPort(port => port
    .Udp("192.168.1.12", 8086)
    .AddModbusAsciiDevice());

builder.AddPort(p => p
    .Serial("COM3")
    .AddModbusRtuDevice());


await Host.CreateDefaultBuilder(args)
    .ConfigureServices(x => x
        .AddEdgeGateway(builder.Build())
        .AddSerial()
        .AddEthernet()
        .AddModbus()
        .AddDemo()
        )
    .RunConsoleAsync();