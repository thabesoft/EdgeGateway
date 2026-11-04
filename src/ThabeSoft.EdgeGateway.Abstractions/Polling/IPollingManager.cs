using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Polling;


public interface IPollingManager
{
    IReadOnlyDictionary<TagId, ITagMonitor> TagMonitors { get; }

    ITagMonitor<T> Register<T>(ITagDescriptor<T> descriptor, ITrigger trigger) where T : notnull;
    void Unregister(TagId id);
}

public static class PollingManagerExtensions
{

    extension(IPollingManager pollingManager)
    {
        public void Unregister(string id) =>
           pollingManager.Unregister(TagId.From(id));
    }

}