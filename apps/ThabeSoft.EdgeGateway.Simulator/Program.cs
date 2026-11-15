using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.DependencyInjection;
using ThabeSoft.EdgeGateway.Simulator.Hosting;

Console.Title = "ModbusSalveSimulator";


await Host.CreateDefaultBuilder(args)
    .ConfigureLogging(x => x.SetMinimumLevel(LogLevel.Debug))
    .ConfigureServices(services => services
        // 模拟器
        .AddEdgeGatewaySimulation()
        .AddEdgeGatewaySimulationHost()
        .AddEdgeGatewayTriggers()
        .AddEdgeGatewayModbusSimulation()

        .AddHostedService<LaunchHostedService>()
    // App
    //.AddApp()
    )
    .RunConsoleAsync();