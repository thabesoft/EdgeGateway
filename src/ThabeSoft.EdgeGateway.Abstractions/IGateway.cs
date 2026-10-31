using EdgeGateway.Port;

namespace EdgeGateway;


public interface IGateway : ILifecycle, IPortManager, IAsyncDisposable;