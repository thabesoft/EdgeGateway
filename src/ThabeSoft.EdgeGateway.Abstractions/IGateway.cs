using ThabeSoft.EdgeGateway.Port;

namespace ThabeSoft.EdgeGateway;


public interface IGateway : ILifecycle, IPortManager, IAsyncDisposable;