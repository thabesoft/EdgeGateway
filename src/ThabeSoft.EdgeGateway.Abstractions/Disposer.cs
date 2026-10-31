namespace ThabeSoft.EdgeGateway;

public sealed class Disposer(Action disposeAction) : IDisposable
{
    public void Dispose() => disposeAction.Invoke();
}

public sealed class CallbackLifecycle : Lifecycle
{
    public Func<CancellationToken, ValueTask>? OnStart { get; init; }
    public Func<CancellationToken, ValueTask>? OnStop { get; init; }
    public Func<ValueTask>? OnDispose { get; init; }


    protected override ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (OnStart is null) return ValueTask.CompletedTask;
        return OnStart.Invoke(cancellationToken);
    }

    protected override ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        if (OnStop is null) return ValueTask.CompletedTask;
        return OnStop.Invoke(cancellationToken);
    }

    protected override ValueTask OnDisposeAsync()
    {
        if (OnDispose is null) return ValueTask.CompletedTask;
        return OnDispose.Invoke();
    }
}