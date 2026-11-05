namespace ThabeSoft.EdgeGateway.Triggers;


internal sealed class SignalTrigger : ITrigger
{
    private readonly SemaphoreSlim _signal = new(0);
    private readonly CancellationTokenSource _cts = new();
    private bool _disposed;


    public void Signal()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _signal.Release();
    }
    public async ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken, _cts.Token);

        await _signal.WaitAsync(linked.Token);
        return true;
    }

    public ValueTask DisposeAsync()
    {
        if (_disposed) return ValueTask.CompletedTask;
        _disposed = true;

        _cts.Cancel();
        _cts.Dispose();
        _signal.Dispose();
        return ValueTask.CompletedTask;
    }
}