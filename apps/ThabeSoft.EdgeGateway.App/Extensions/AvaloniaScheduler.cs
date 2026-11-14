using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Concurrency;
using System.Reactive.Linq;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System.Reactive.Linq;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

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
