using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.Ports;


public interface ISimulationPort : ILifecycle, IAsyncDisposable
{
    ValueTask<ISimulationDevice> AddDeviceAsync(ISimulationDeviceDescriptor simulationDeviceDescriptor, CancellationToken cancellationToken = default);
    ValueTask RemoveDeviceAsync(ISimulationDeviceId id, CancellationToken cancellationToken = default);
}