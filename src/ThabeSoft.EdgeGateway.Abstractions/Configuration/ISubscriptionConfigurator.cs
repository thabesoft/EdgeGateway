using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.EdgeGateway.Configuration;

public interface ISubscriptionConfigurator<T> : IDeviceConfigurator
    where T : notnull
{
    ISubscriptionConfigurator<T> Observable(Action<IObservable<Reading<T>>> configureObservable);
}

public static class SubscriptionConfiguratorExtensions
{
    extension<T>(ISubscriptionConfigurator<T> configurator) where T : notnull
    {
        public ISubscriptionConfigurator<T> Subscribe(Action<Reading<T>> onNext, Action<Exception>? onError = null, Action? onCompleted = null) =>
            configurator.Observable(x => x.Subscribe(new Observer<T>() { Next = onNext, Error = onError, Completed = onCompleted }));
    }

    private sealed class Observer<T> : IObserver<Reading<T>>
        where T : notnull
    {
        public Action? Completed { get; init; }
        public Action<Reading<T>>? Next { get; init; }
        public Action<Exception>? Error { get; init; }


        public void OnNext(Reading<T> value) => Next?.Invoke(value);
        public void OnCompleted() => Completed?.Invoke();
        public void OnError(Exception error) => Error?.Invoke(error);
    }
}