using ThabeSoft.EdgeGateway.Addressing;

namespace ThabeSoft.EdgeGateway.Tags;


public interface ITagMonitor
{
    TagId Id { get; }
    IDataAddress Address { get; }


    DateTimeOffset UpdateTime { get; }
    object? Value { get; }
    TagValueState State { get; }
}

public interface ITagMonitor<T> : ITagMonitor
    where T : notnull
{
    event Action<T> Updated;
    new T? Value { get; }


#if NETCOREAPP
    object? ITagMonitor.Value => Value;
#endif
}