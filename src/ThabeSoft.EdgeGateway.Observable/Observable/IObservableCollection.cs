namespace ThabeSoft.EdgeGateway.Observable;


public interface IObservableCollection<T> : IReadOnlyCollection<T>
{
    IObservable<T> Added { get; }
    IObservable<T> Removed { get; }
    IObservable<CollectionCountChanged> CountChanged { get; }
    IObservable<CollectionReset> Reset { get; }



    bool TryAdd(T item);
    bool TryRemove(T item);
    void Clear();
}


public readonly record struct CollectionReset;
public readonly record struct CollectionCountChanged(int OldValue, int NewValue);