using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Devices.Modbus;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.Utilities;
using ThabeSoft.EdgeGateway.ValueGenerators;

namespace ThabeSoft.EdgeGateway.Devices;


internal abstract class SimulationDeviceBase(
        ModbusSimulationDeviceId id,
        TriggerManager triggerManager,
        ILogger? logger = null
    ) : ISimulationDevice
{
    private readonly ILifecycleStateMachine _stateMachine = Lifecycle.StrictStateMachine();
    private readonly TaskManager _taskManager = new();
    //private readonly ConcurrentDictionary<Guid, ITrigger> _triggers = [];
    private ILogger _logger = logger ?? NullLogger.Instance;


    public ISimulationDeviceId Id => id;
    public LifecycleState State => _stateMachine.State;


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(_taskManager.StartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(_taskManager.StopAsync, cancellationToken);
    public ValueTask DisposeAsync() => _stateMachine.DisposeAsync(async () =>
    {
        await _taskManager.DisposeAsync();
    });


    public async ValueTask<IDisposable> ValueAsync<T>(TagDescriptor<T> tagDescriptor, TriggerDescriptor triggerDescriptor, IValueGenerator<T> generator, CancellationToken cancellationToken = default)
        where T : notnull
    {
        if (tagDescriptor.Address is not ModbusDataAddress dataAddrss)
            throw new InvalidOperationException("不支持的标签");

        if (tagDescriptor.Codec is not IRequiredBits requiredBits)
            throw new InvalidOperationException("不支持的标签");

        var scheduler = await triggerManager.GetSchedulerAsync(triggerDescriptor, cancellationToken);
        return scheduler.Subcribe(() =>
        {
            var value = generator.Next();
            _logger.LogTrace("设备({Id}) 已生成值({Value})", Id, value);

            OnValue(tagDescriptor, value);
        });
    }

    protected abstract void OnValue<T>(TagDescriptor<T> tag, T value) where T : notnull;
}


internal class TriggerManager(ITriggerResolver triggerFactorySelector, ILoggerFactory loggerFactory)
{
    private readonly ConcurrentDictionary<TriggerDescriptor, TriggerScheduler> _entries = [];

    public async ValueTask<TriggerScheduler> GetSchedulerAsync(TriggerDescriptor triggerDescriptor, CancellationToken cancellationToken)
    {
        var scheduler = _entries.GetOrAdd(triggerDescriptor, x =>
        {
            var trigger = triggerFactorySelector.Resolve(x);
            return new TriggerScheduler(trigger, loggerFactory.CreateLogger<TriggerScheduler>());
        });

        await scheduler.StartAsync(cancellationToken);
        return scheduler;
    }

}