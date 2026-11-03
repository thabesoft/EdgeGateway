using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.App.Designs;

public sealed class DesignTagMonitor<T>(TagName tagName, ITag<T> tag) : ITagMonitor<T>
    where T : notnull
{
    public event Action<T>? Updated;
    public TagName Name => tagName;
    public ITag<T> Tag => tag;


    public T? Value { get; } = default;
    public DateTimeOffset UpdateTime { get; } = DateTimeOffset.MinValue;
    public TagValueState State { get; } = TagValueState.Bad;
}
