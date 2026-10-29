namespace EdgeGateway.Tag.Polling;


public interface IPollingScheduler
{
    IReadOnlyDictionary<ITag, ITagMonitor> Monitors { get; }
    ITagMonitor<T> Register<T>(ITag<T> tag, IPollingTrigger pollingTrigger) where T : notnull;
    void Unregister<T>(ITag<T> tag) where T : notnull;


    ValueTask StartAsync(CancellationToken cancellationToken = default);
    ValueTask StopAsync(CancellationToken cancellationToken = default);
}
