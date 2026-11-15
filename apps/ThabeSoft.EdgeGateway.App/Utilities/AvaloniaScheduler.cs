using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Concurrency;
using System.Reactive.Linq;

namespace ThabeSoft.EdgeGateway.App.Utilities;

internal static class AvaloniaObservableExtensions
{
    extension<T>(IObservable<T> observable)
    {
        public IObservable<T> ObserveOnUI()
        {
            return observable.ObserveOn(AvaloniaScheduler.Instance);
        }
    }
}
