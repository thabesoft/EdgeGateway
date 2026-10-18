using EdgeGateway.Ports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EdgeGateway.Console.Services;

internal sealed class CommandProcesserHostedService(IAppScope appScope, ILogger<CommandProcesserHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var gateway = appScope.ServiceProvider.GetRequiredService<ILifetime>();
        var portManager = appScope.ServiceProvider.GetRequiredService<IPortManager>();

        await gateway.StartAsync(stoppingToken);


        while(true)
        {
            var input = System.Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(input)) continue;

            if(input.Equals("list", StringComparison.OrdinalIgnoreCase))
            {
                var str = string.Join(Environment.NewLine, portManager.Ports.Values.Select(x => x.Address));
                WriteLine(str);
            }
        }
    }


    private static void Write(string str) => System.Console.Write(str);
    private static void WriteLine(string str) => System.Console.WriteLine(str);
}