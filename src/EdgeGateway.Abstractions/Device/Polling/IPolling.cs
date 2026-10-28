using System.Collections.Concurrent;

namespace EdgeGateway.Device.Polling;


public interface IPollingTrigger : ILifecycle, IAsyncDisposable
{
    IDisposable Subscribe(Func<CancellationToken, ValueTask> onTick);
}
