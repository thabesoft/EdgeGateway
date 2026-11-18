using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;

namespace ThabeSoft.EdgeGateway.Collections;


internal sealed class LifecycleKeyedCollectionFactory(ILoggerFactory loggerFactory) : ILifecycleKeyedCollectionFactory
{
    public ILifecycleKeyedCollection<TKey, TLifecycle> Create<TKey, TLifecycle>(ILifecycleStateable hostState)
        where TKey : notnull
        where TLifecycle : ILifecycle, IAsyncDisposable
    {
        var logger = loggerFactory.CreateLogger<LifecycleKeyedCollection<TKey, TLifecycle>>();
        return new LifecycleKeyedCollection<TKey, TLifecycle>(hostState, logger);
    }
}