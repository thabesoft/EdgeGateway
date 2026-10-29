namespace EdgeGateway.Device.Polling;


public interface IPollingTrigger : ILifecycle, IAsyncDisposable
{
    IDisposable Subscribe(PollingTriggerOnTickHandler onTick);
}


public delegate ValueTask PollingTriggerOnTickHandler(CancellationToken cancellationToken = default);