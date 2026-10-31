using EdgeGateway.Byte.Codecs;
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


        var tagx = ModbusTag.HoldingRegister(1, 0, WordCodec.BigEndian);
        var tagy = ModbusTag.HoldingRegister(1, 1, WordCodec.BigEndian);

        var monitorx = a.Register(tagx, new FixedIntervalTrigger(TimeSpan.FromSeconds(1)));
        monitorx.Updated += x => 
            Console.WriteLine($"更新了X: {x}");

        var monitory = a.Register(tagy, new FixedIntervalTrigger(TimeSpan.FromSeconds(1)));
        monitory.Updated += y =>
            Console.WriteLine($"更新了X: {y}");



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