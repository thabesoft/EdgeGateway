using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;

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
