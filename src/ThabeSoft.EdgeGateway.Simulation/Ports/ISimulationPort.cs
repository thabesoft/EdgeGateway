using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


public interface ISimulationPort : ILifecycle, ILifecycleStateable, IDisposable, IAsyncDisposable
{
    ConnectionEndpoint Endpoint { get; }

    ValueTask<ISimulationDevice> AddDeviceAsync(SimulationDeviceDescriptor simulationDeviceDescriptor, CancellationToken cancellationToken = default);
    ValueTask RemoveDeviceAsync(ISimulationDeviceId id, CancellationToken cancellationToken = default);
}