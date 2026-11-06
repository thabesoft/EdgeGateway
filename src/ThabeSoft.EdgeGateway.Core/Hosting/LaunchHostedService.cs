using Microsoft.Extensions.Hosting;

namespace ThabeSoft.EdgeGateway.Hosting;


internal sealed class LaunchHostedService(IGateway gateway) : IHostedService
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