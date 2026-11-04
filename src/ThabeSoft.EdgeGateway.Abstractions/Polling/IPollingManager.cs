using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Polling;


public interface IPollingManager
{
    IReadOnlyDictionary<TagName, ITagMonitor> TagMonitors { get; }

    ITagMonitor<T> Register<T>(ITagDescriptor<T> descriptor, ITrigger trigger) where T : notnull;
    void Unregister(TagName tagName);
}

public static class PollingManagerExtensions
{

    extension(IPollingManager pollingManager)
    {
        public void Unregister(string tagName) =>
           pollingManager.Unregister(TagName.From(tagName));
    }

}