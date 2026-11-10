namespace ThabeSoft.EdgeGateway.Utilities;


public sealed class Disposer : IDisposable
{
    private readonly Action _action;

    public Disposer(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        _action = action;
    }
    public static Disposer From(Action action) => new(action);


    public void Dispose() => _action.Invoke();
}