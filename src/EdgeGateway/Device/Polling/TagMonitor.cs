using EdgeGateway.Tag;

namespace EdgeGateway.Device.Polling;

public sealed class TagMonitor<T>(ITag<T> tag) : ITagMonitor<T>
    where T : notnull
{
    public ITag<T> Tag => tag;
    public DateTimeOffset UpdateTime { get; private set; }
    public T? Value { get; private set; }


#if NETSTANDARD
    ITag ITagMonitor.Tag => Tag;
    object? ITagMonitor.Value => Value;
#endif

    public void Update(T value)
    {
        Value = value;
        UpdateTime = DateTimeOffset.UtcNow;
    }
}