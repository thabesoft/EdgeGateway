using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Polling;


internal sealed class TagMonitor<T>(TagName tagName, ITagDescriptor<T> tag) : ITagMonitor<T>
    where T : notnull
{
    public event Action<T>? Updated;

    public ITagDescriptor<T> Tag => tag;
    public TagName Name => tagName;


    public DateTimeOffset UpdateTime { get; private set; }
    public T? Value { get; private set; }
    public TagValueState State { get; private set; } = TagValueState.Unknown;


#if NETSTANDARD
    ITagDescriptor ITagMonitor.Tag => Tag;
    object? ITagMonitor.Value => Value;

#endif

    public void SetValue(T value)
    {
        Value = value;
        UpdateTime = DateTimeOffset.UtcNow;
        State = TagValueState.Good;

        Updated?.Invoke(value);
    }

    public void SetError(Exception exception)
    {
        UpdateTime = DateTimeOffset.UtcNow;
        State = TagValueState.Bad;
    }
}