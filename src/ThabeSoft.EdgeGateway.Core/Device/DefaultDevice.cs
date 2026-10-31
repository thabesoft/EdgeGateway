using EdgeGateway.Polling;
using EdgeGateway.Polling.Triggers;
using EdgeGateway.Protocol;
using EdgeGateway.Tag;

namespace EdgeGateway.Device;


internal sealed class DefaultDevice(IPollingScheduler pollingScheduler) : IDevice
{
    public required DeviceId Id { get; init; }
    public required ProtocolType ProtocolType { get; init; }
    public bool IsEnabled { get; private set; }
    public IReadOnlyDictionary<TagName, ITagMonitor> Monitors => pollingScheduler.Monitors;


    public async ValueTask EnableAsync(CancellationToken cancellationToken = default)
    {
        IsEnabled = true;
        await pollingScheduler.StartAsync(cancellationToken);
    }
    public async ValueTask DisableAsync(CancellationToken cancellationToken = default)
    {
        IsEnabled = false;
        await pollingScheduler.StopAsync(cancellationToken);
    }

    public ITagMonitor<T> Register<T>(TagName tagName, ITag<T> tag, IPollingTrigger pollingTrigger) where T : notnull =>
        pollingScheduler.Register(tagName, tag, pollingTrigger);
    public void Unregister(TagName tagName) =>
        pollingScheduler.Unregister(tagName);


    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
