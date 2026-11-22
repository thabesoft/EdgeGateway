using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Configuration.Diff;

namespace ThabeSoft.EdgeGateway.Hosting;


internal sealed class LaunchHostedService(
        IGateway gateway,
        GatewayConfigApplier configApplier,
        ILogger<LaunchHostedService> logger
    ) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        System.Diagnostics.Debug.WriteLine(typeof(GatewayConfig).AssemblyQualifiedName);


        logger.LogTrace("网关启动中");

        await gateway.StartAsync(cancellationToken);
        configApplier.Enable();

        logger.LogTrace("网关已启动");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("网关关闭中");

        configApplier.Disable();
        await gateway.StopAsync(cancellationToken);

        logger.LogTrace("网关已关闭");
    }
}