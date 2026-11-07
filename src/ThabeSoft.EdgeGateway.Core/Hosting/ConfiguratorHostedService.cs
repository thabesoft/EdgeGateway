using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.Configuration;

namespace ThabeSoft.EdgeGateway.Hosting;


internal class ConfiguratorHostedService(IGateway gateway, IEnumerable<GatewayConfiguratorHandler> configuratorHandlers) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var builder = new GatewayConfigurator(gateway);

        foreach (var i in configuratorHandlers)
        {
            i.Invoke(builder);
        }
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
