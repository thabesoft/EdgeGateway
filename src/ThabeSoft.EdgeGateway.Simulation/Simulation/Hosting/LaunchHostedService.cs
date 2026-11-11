using Microsoft.Extensions.Hosting;

namespace ThabeSoft.EdgeGateway.Simulation.Hosting;


internal sealed class LaunchHostedService(ISimulator simulatorHost) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await simulatorHost.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await simulatorHost.StopAsync(cancellationToken);
    }
}