using EdgeGateway.Port;

namespace EdgeGateway;


public interface IGateway : IGatewayLifecycle, IPortManager, IAsyncDisposable;