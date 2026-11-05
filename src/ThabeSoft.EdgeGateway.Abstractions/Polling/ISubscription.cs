using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Polling;


public interface ISubscription : ILifecycle, IAsyncDisposable
{
    ITag Tag { get; }
}

public interface ISubscription<T> : ISubscription
    where T : notnull
{
    new ITag<T> Tag { get; }
}