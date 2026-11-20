using Microsoft.Extensions.Logging;

namespace ThabeSoft.Lifecycles.Collections;


internal sealed class LifecycleKeyedCollectionFactory(ILoggerFactory loggerFactory) : ILifecycleKeyedCollectionFactory
{
    public ILifecycleKeyedCollection<TKey, TLifecycle> Create<TKey, TLifecycle>(ILifecycleStateable hostState)
        where TKey : notnull
        where TLifecycle : ILifecycle
    {
        var logger = loggerFactory.CreateLogger<LifecycleKeyedCollection<TKey, TLifecycle>>();
        return new LifecycleKeyedCollection<TKey, TLifecycle>(hostState, logger);
    }
}