using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;

namespace ThabeSoft.EdgeGateway.Simulation.Networks;

public interface IModbusSimulationNetwork : ILifecycle, IAsyncDisposable
{
    ValueTask<IModbusSimulatedSlave> AddSlaveAsync(byte unitId, CancellationToken cancellationToken = default);
    ValueTask RemoveSalveAsync(byte unitId, CancellationToken cancellationToken = default);
}