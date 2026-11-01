using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway;


public interface IGateway : ILifecycle, IPortManager, IAsyncDisposable;