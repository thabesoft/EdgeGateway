using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway;
using ThabeSoft.EdgeGateway.Simulator;

Console.Title = "ModbusSalveSimulator";


await Host.CreateDefaultBuilder(args)
    .ConfigureLogging(x => x.SetMinimumLevel(LogLevel.Debug))
    .ConfigureServices(services => services
        // 模拟器
        .AddSimulation()
        .AddTriggers()
        .AddSimulationHost()
        .AddModbusSimulation()

        .AddHostedService<LaunchHostedService>()
    // App
    //.AddApp()
    )
    .RunConsoleAsync();