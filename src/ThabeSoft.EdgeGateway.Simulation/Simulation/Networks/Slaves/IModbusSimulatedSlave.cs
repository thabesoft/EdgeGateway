using NModbus;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Simulation.ValueGenerators;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;

public interface IModbusSimulatedSlave : IStatefulLifecycle, IAsyncDisposable
{
    internal IModbusSlave Slave { get; }
    IDisposable Value<T>(TagDescriptor<T> tagDescriptor, ITriggerDescriptor triggerDescriptor, IValueGenerator<T> generator) where T : notnull;
}