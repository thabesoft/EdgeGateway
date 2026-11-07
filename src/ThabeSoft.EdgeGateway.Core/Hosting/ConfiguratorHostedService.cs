using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Options;

namespace ThabeSoft.EdgeGateway.Hosting;


internal class ConfiguratorHostedService(IGateway gateway, IOptions<GatewayOptions> options, IEnumerable<GatewayConfiguratorHandler> configuratorHandlers) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (options.Value.IsDisabledConfigurator)
            return;


        var builder = new GatewayConfigurator(gateway);
        foreach (var i in configuratorHandlers) i.Invoke(builder);
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
