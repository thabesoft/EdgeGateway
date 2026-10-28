using EdgeGateway.Tag;

namespace EdgeGateway.Device.Polling;


public interface ITagMonitor<T> : ITagMonitor
    where T : notnull
{
    new ITag<T> Tag { get; }
    new T? Value { get; }


#if NETCOREAPP
    ITag ITagMonitor.Tag => Tag;
    object? ITagMonitor.Value => Value;
#endif
}

public interface ITagMonitor
{
    ITag Tag { get; }

    DateTimeOffset UpdateTime { get; }
    object? Value { get; }
}
