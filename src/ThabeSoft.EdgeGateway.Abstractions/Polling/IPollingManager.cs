using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Polling;


public interface IPollingManager
{
    IReadOnlyDictionary<TagName, ITagMonitor> Monitors { get; }

    ITagMonitor<T> Register<T>(TagName tagName, ITag<T> tag, ITrigger trigger) where T : notnull;
    void Unregister(TagName tagName);
}

public static class PollingManagerExtensions
{

    extension(IPollingManager pollingManager)
    {
        public ITagMonitor<T> Register<T>(string tagName, ITag<T> tag, ITrigger trigger) where T : notnull =>
            pollingManager.Register(TagName.From(tagName), tag, trigger);

        public void Unregister(string tagName) =>
           pollingManager.Unregister(TagName.From(tagName));
    }

}