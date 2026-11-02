namespace ThabeSoft.EdgeGateway;

public sealed class Disposer(Action disposeAction) : IDisposable
{
    public void Dispose() => disposeAction.Invoke();
}