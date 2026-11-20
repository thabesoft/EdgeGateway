namespace ThabeSoft.Lifecycles.Collections;


public interface ILifecycleKeyedCollectionFactory
{
    ILifecycleKeyedCollection<TKey, TLifecycle> Create<TKey, TLifecycle>(ILifecycleStateable hostState)
        where TKey : notnull
        where TLifecycle : ILifecycle;
}