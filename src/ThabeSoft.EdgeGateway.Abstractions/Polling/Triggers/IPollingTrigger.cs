namespace ThabeSoft.EdgeGateway.Polling.Triggers;


public interface IPollingTrigger : ILifecycle, IAsyncDisposable
{
    IDisposable Subscribe(PollingTriggerOnTickHandler onTick);
}


public delegate ValueTask PollingTriggerOnTickHandler(CancellationToken cancellationToken = default);