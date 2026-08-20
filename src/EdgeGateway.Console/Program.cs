using EdgeGateway.Console;
using EdgeGateway.Console.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.Title = "EdgeGateway";

await Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(x => x.AddJsonFile("gateway.json", true, true))
    .ConfigureServices(x =>
    {
        x.AddEdgeGateway();
        x.AddSingleton<IAppScope, AppScope>();

        x.AddHostedService<CommandProcesserHostedService>();
    })
    .RunConsoleAsync();
