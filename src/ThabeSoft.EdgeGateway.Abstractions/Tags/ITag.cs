using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.Tags;


public interface ITag
{
    TagId Id { get; }
    IDataAddress Address { get; }

    DateTimeOffset Timestamp { get; }
    TagQuality Quality { get; }
    object? Value { get; }


    IDisposable Subscribe(Action<object> callback);
}

public interface ITag<T> : ITag
    where T : notnull
{
    new T? Value { get; }

    IDisposable Subscribe(Action<T> callback);
}