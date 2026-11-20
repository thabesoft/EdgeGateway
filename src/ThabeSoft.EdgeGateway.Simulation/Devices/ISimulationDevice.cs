using ThabeSoft.EdgeGateway.ValueGenerators;
using ThabeSoft.Lifecycles;
using ThabeSoft.Protocols;
using ThabeSoft.Triggers;

namespace ThabeSoft.EdgeGateway.Devices;


public interface ISimulationDevice : ILifecycle, ILifecycleStateable, IAsyncDisposable
{
    ISimulationDeviceId Id { get; }
    ValueTask<IDisposable> ValueAsync<T>(DataAccessDescriptor<T> tagDescriptor, TriggerDescriptor triggerDescriptor, IValueGenerator<T> generator, CancellationToken cancellationToken = default) where T : notnull;
}