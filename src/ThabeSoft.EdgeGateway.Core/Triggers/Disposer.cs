namespace ThabeSoft.EdgeGateway.Triggers;

internal sealed class Disposer(Action disposeAction) : IDisposable
{
    public void Dispose() => disposeAction.Invoke();
}