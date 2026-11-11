using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Devices.Modbus;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.Utilities;
using ThabeSoft.EdgeGateway.ValueGenerators;

namespace ThabeSoft.EdgeGateway.Devices;


internal abstract class SimulationDeviceBase(
        ModbusSimulationDeviceDescriptor descriptor,
        ITriggerFactorySelector triggerFactorySelector,
        ILogger? logger = null
    ) : ISimulationDevice
{
    private readonly IStrictLifecycleStateMachine _stateMachine = Lifecycle.StrictStateMachine();
    private readonly TaskManager _taskManager = new();
    private readonly ConcurrentDictionary<Guid, ITrigger> _triggers = [];
    private ILogger _logger = logger ?? NullLogger.Instance;


    public ISimulationDeviceId Id => descriptor.Id;
    public LifecycleState State => _stateMachine.State;


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(_taskManager.StartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(_taskManager.StopAsync, cancellationToken);
    public ValueTask DisposeAsync() => _stateMachine.DisposeAsync(async () =>
    {
        foreach (var i in _triggers.Values)
            await i.DisposeAsync();

        await _taskManager.DisposeAsync();
    });


    public IDisposable Value<T>(TagDescriptor<T> tagDescriptor, ITriggerDescriptor triggerDescriptor, IValueGenerator<T> generator)
        where T : notnull
    {
        if (tagDescriptor.Address is not ModbusDataAddress dataAddrss)
            throw new InvalidOperationException("不支持的标签");

        if (tagDescriptor.Codec is not IRequiredBits requiredBits)
            throw new InvalidOperationException("不支持的标签");

        var trigger = triggerFactorySelector.Create(triggerDescriptor);
        _triggers[Guid.NewGuid()] = trigger;


        return _taskManager.AddTask(async ct =>
        {
            _logger.LogTrace("添加生成值");

            while (!ct.IsCancellationRequested)
            {
                if (!await trigger.WaitForNextTickAsync(ct))
                    return;

                var value = generator.Next();
                _logger.LogTrace("设备({Id}) 已生成值({Value})", Id, value);

                await OnValueAsync(tagDescriptor, value, ct);
            }
        });
    }

    protected abstract ValueTask OnValueAsync<T>(TagDescriptor<T> tag, T value, CancellationToken cancellationToken) where T : notnull;
}
