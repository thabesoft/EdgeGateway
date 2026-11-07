using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ThabeSoft.EdgeGateway.Options;

namespace ThabeSoft.EdgeGateway.Hosting;


internal sealed class LaunchHostedService(IGateway gateway, IOptions<GatewayOptions> options) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (options.Value.IsDisabledAutoLauch)
            return;

        await gateway.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await gateway.StopAsync(cancellationToken);
    }
}