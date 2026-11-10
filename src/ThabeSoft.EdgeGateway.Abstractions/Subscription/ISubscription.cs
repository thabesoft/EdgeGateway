using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.Subscription;


public interface ISubscription : ILifecycleWithState, IObservable<Reading>
{
    SubscriptionId Id { get; }
    IDataAddress Address { get; }
    Reading? Latest { get; }
}

public interface ISubscription<T> : ISubscription, IObservable<Reading<T>>
    where T : notnull
{
    new Reading<T>? Latest { get; }
}


public static class SubscriptionExtensions
{
    extension<T>(ISubscription<T> configurator) where T : notnull
    {
        public IDisposable Subscribe(Action<Reading<T>> onNext, Action<Exception>? onError = null, Action? onCompleted = null) =>
            configurator.Subscribe(new Observer<Reading<T>>() { Next = onNext, Error = onError, Completed = onCompleted });
    }

    extension(ISubscription configurator)
    {
        public IDisposable Subscribe(Action<Reading> onNext, Action<Exception>? onError = null, Action? onCompleted = null) =>
            configurator.Subscribe(new Observer<Reading>() { Next = onNext, Error = onError, Completed = onCompleted });
    }


    private sealed class Observer<T> : IObserver<T>
        where T : notnull
    {
        public Action? Completed { get; init; }
        public Action<T>? Next { get; init; }
        public Action<Exception>? Error { get; init; }


        public void OnNext(T value) => Next?.Invoke(value);
        public void OnCompleted() => Completed?.Invoke();
        public void OnError(Exception error) => Error?.Invoke(error);
    }
}