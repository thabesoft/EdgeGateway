using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Polling;


public interface ITagMonitor
{
    TagName Name { get; }
    ITagDescriptor Tag { get; }


    DateTimeOffset UpdateTime { get; }
    object? Value { get; }
    TagValueState State { get; }
}

public interface ITagMonitor<T> : ITagMonitor
    where T : notnull
{
    event Action<T> Updated;

    new ITagDescriptor<T> Tag { get; }
    new T? Value { get; }


#if NETCOREAPP
    ITagDescriptor ITagMonitor.Tag => Tag;
    object? ITagMonitor.Value => Value;
#endif
}