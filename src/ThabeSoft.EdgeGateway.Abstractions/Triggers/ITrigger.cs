namespace ThabeSoft.EdgeGateway.Triggers;


public interface ITrigger : IAsyncDisposable
{
    ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default);
}