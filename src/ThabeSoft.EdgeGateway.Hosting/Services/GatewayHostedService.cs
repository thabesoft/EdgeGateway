using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.Configs;

namespace ThabeSoft.EdgeGateway.Services;


internal sealed class GatewayHostedService(IGateway gateway, GatewayConfig config) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await gateway.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await gateway.StopAsync(cancellationToken);
    }
}
