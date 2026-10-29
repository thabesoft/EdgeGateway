using EdgeGateway.Tag;

namespace EdgeGateway.Polling;


public interface ITagMonitor<T> : ITagMonitor
    where T : notnull
{
    event Action<T> Updated;

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



public sealed class TagMonitor<T>(ITag<T> tag) : ITagMonitor<T>
    where T : notnull
{
    public event Action<T>? Updated;
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

        Updated?.Invoke(value);
    }
}