using EdgeGateway.Console;
using EdgeGateway.Console.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.Title = "EdgeGateway";

await Host.CreateDefaultBuilder()
    .ConfigureServices(x =>
    {
        x.AddEdgeGateway();
        x.AddEdgeGatewaySerialPort();
        x.AddEdgeGatewayEthernet();
        x.AddEdgeGatewayModbus();
        x.AddEdgeGatewayOptions();


        x.AddSingleton<IAppScope, AppScope>();
        x.AddHostedService<CommandProcesserHostedService>();
    })
    .RunConsoleAsync();
