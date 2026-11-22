using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.Configuration.Diff;

namespace ThabeSoft.EdgeGateway.Hosting;


internal sealed class LaunchHostedService(IGateway gateway, GatewayConfigApplier configApplier) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await gateway.StartAsync(cancellationToken);
        configApplier.Enable();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        configApplier.Disable();
        await gateway.StopAsync(cancellationToken);
    }
}