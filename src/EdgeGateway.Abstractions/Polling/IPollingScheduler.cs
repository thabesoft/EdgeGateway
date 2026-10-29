using EdgeGateway.Polling.Triggers;
using EdgeGateway.Tag;

namespace EdgeGateway.Polling;


public interface IPollingManager
{
    IReadOnlyDictionary<ITag, ITagMonitor> Monitors { get; }

    ITagMonitor<T> Register<T>(ITag<T> tag, IPollingTrigger pollingTrigger) where T : notnull;
    void Unregister<T>(ITag<T> tag) where T : notnull;
}

public interface IPollingScheduler : IPollingManager, ILifecycle;