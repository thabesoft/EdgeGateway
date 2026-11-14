namespace ThabeSoft.EdgeGateway.Lifecycles;


interface IDisposeObservable
{
    IDisposable Subscribe(Func<ValueTask> handler);
}