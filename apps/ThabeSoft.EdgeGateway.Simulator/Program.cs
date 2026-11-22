using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Simulation.DependencyInjection;
using ThabeSoft.EdgeGateway.Simulator.Hosting;
using ThabeSoft.Triggers.DependencyInjection;

Console.Title = "ModbusSalveSimulator";


await Host.CreateDefaultBuilder(args)
    .ConfigureLogging(x => x.SetMinimumLevel(LogLevel.Debug))
    .ConfigureServices(services => services
        .AddTriggers()
        // 模拟器
        .AddEdgeGatewaySimulation()
        .AddEdgeGatewaySimulationHost()
        .AddEdgeGatewayModbusSimulation()
        .AddHostedService<LaunchHostedService>()
    )
    .RunConsoleAsync();