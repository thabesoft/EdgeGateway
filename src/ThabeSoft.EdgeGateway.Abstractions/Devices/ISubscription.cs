using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Devices;


public interface ISubscription : ILifecycle, IAsyncDisposable
{
    ITag Tag { get; }

    // 这个方法应该属于Tag 
    IDisposable Subscribe(Action<object> callback);
}

public interface ISubscription<T> : ISubscription
    where T : notnull
{
    new ITag<T> Tag { get; }
}