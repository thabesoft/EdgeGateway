using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EdgeGateway;


internal sealed class GatewayHostedService(IGateway gateway, ILogger<GatewayHostedService> logger)
    : IHostedService, IAsyncDisposable
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
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