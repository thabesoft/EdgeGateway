using Microsoft.Extensions.Hosting;

namespace ThabeSoft.EdgeGateway.Configuration.Rloader;


internal sealed class ConfigFileReloadHostedService(
        IConfigSerializer configSerializer,
        ConfigFileWatcher configFileWatcher,
        ConfigApplier configManager
    ) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        configFileWatcher.Changed += ConfigFileWatcher_Changed;
        await ConfigFileWatcher_Changed(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        configFileWatcher.Changed -= ConfigFileWatcher_Changed;
        return Task.CompletedTask;
    }

    private async Task ConfigFileWatcher_Changed(CancellationToken cancellationToken)
    {
        var config = await configSerializer.DeserializeFileAsync(configFileWatcher.FilePath, cancellationToken);
        await configManager.ApplyAsync(config, cancellationToken);
    }
}