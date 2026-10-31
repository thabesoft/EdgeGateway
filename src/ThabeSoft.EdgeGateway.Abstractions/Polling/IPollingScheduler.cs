using ThabeSoft.EdgeGateway.Polling.Triggers;
using ThabeSoft.EdgeGateway.Tag;

namespace ThabeSoft.EdgeGateway.Polling;


public interface IPollingManager
{
    IReadOnlyDictionary<TagName, ITagMonitor> Monitors { get; }

    ITagMonitor<T> Register<T>(TagName tagName, ITag<T> tag, IPollingTrigger pollingTrigger) where T : notnull;
    void Unregister(TagName tagName);
}

public interface IPollingScheduler : IPollingManager, ILifecycle;