namespace ThabeSoft.EdgeGateway.Lifecycles;

internal sealed class CallbackLifecycle : ILifecycle, IAsyncDisposable
{
    public Func<CancellationToken, ValueTask>? OnStart { get; init; }
    public Func<CancellationToken, ValueTask>? OnStop { get; init; }
    public Func<ValueTask>? OnDispose { get; init; }



    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        if (OnStart is null) return ValueTask.CompletedTask;
        return OnStart.Invoke(cancellationToken);
    }
    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        if (OnStop is null) return ValueTask.CompletedTask;
        return OnStop.Invoke(cancellationToken);
    }
    public ValueTask DisposeAsync()
    {
        if (OnDispose is null) return ValueTask.CompletedTask;
        return OnDispose.Invoke();
    }
}