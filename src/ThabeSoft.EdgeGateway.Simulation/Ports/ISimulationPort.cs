using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.Ports;


public interface ISimulationPort : ILifecycle, ILifecycleStateable, IAsyncDisposable
{
    ValueTask<ISimulationDevice> AddDeviceAsync(SimulationDeviceDescriptor simulationDeviceDescriptor, CancellationToken cancellationToken = default);
    ValueTask RemoveDeviceAsync(ISimulationDeviceId id, CancellationToken cancellationToken = default);
}