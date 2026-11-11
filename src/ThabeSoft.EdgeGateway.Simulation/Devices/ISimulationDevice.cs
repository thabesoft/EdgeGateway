using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.ValueGenerators;

namespace ThabeSoft.EdgeGateway.Devices;


public interface ISimulationDevice : ILifecycle, ILifecycleStateable, IAsyncDisposable
{
    ISimulationDeviceId Id { get; }
    IDisposable Value<T>(TagDescriptor<T> tagDescriptor, ITriggerDescriptor triggerDescriptor, IValueGenerator<T> generator) where T : notnull;
}