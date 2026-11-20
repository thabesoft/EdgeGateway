using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.Lifecycles;
using ThabeSoft.Transports;

namespace ThabeSoft.EdgeGateway.Ports;


public interface ISimulationPort : ILifecycle, ILifecycleStateable, IAsyncDisposable
{
    ConnectionEndpoint Endpoint { get; }

    ValueTask<ISimulationDevice> AddDeviceAsync(SimulationDeviceDescriptor simulationDeviceDescriptor, CancellationToken cancellationToken = default);
    ValueTask RemoveDeviceAsync(ISimulationDeviceId id, CancellationToken cancellationToken = default);
}