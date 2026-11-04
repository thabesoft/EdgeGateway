using ThabeSoft.EdgeGateway.Addressing;

namespace ThabeSoft.EdgeGateway.Tags;


internal sealed class TagMonitor<T>(TagDescriptor<T> tag) : ITagMonitor<T>
    where T : notnull
{
    public event Action<T>? Updated;


    public TagId Id => tag.Id;
    public IDataAddress Address => tag.Address;


    public DateTimeOffset UpdateTime { get; private set; }
    public T? Value { get; private set; }
    public TagValueState State { get; private set; } = TagValueState.Unknown;


#if NETSTANDARD
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