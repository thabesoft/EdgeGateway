using EdgeGateway.Modbus.Tag;
using EdgeGateway.Polling;
using EdgeGateway.Polling.Triggers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EdgeGateway.Services;


internal sealed class GatewayHostedService(IGateway gateway, ILogger<GatewayHostedService> logger)
    : IHostedService, IAsyncDisposable
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var a = gateway.Ports.Values.SelectMany(x => x.Devices.Values)
            .OfType<IPollingManager>()
            .FirstOrDefault();


        var tag = ModbusTag.Coil(1, 100);
        var monitor = a.Register(tag, new FixedIntervalTrigger(TimeSpan.FromSeconds(1)));
        monitor.Updated += x => 
            Console.WriteLine($"更新了{x}");



        logger.LogInformation("Starting gateway...");
        await gateway.StartAsync(cancellationToken);
        logger.LogInformation("Gateway started.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping gateway...");
        await gateway.StopAsync(cancellationToken);
        logger.LogInformation("Gateway stopped.");
    }

    public async ValueTask DisposeAsync()
    {
        await gateway.DisposeAsync();
    }
}