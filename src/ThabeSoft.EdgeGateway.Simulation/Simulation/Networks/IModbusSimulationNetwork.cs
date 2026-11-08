using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;

namespace ThabeSoft.EdgeGateway.Simulation.Networks;

public interface IModbusSimulationNetwork : ILifecycle, IAsyncDisposable
{
    IModbusSimulatedSlave AddSlave(byte unitId);
}