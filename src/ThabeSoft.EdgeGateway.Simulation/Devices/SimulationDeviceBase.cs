using Microsoft.Extensions.Logging;
using ThabeSoft.Codecs;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Utilities;
using ThabeSoft.EdgeGateway.ValueGenerators;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Primitives;
using ThabeSoft.Protocols;
using ThabeSoft.Protocols.Modbus;
using ThabeSoft.Triggers;

namespace ThabeSoft.EdgeGateway.Simulation.Devices;


internal abstract class SimulationDeviceBase(in SimulationDeviceBaseArgs args) : ISimulationDevice
{
    private DisposeFlag _disposeFlag = new();

    private readonly ILifecycleStateMachine _stateMachine = args.LifecycleStateMachineFactory.Create(args.DeviceStateMachine);
    private readonly TaskManager _taskManager = args.TaskManager;
    private readonly TriggerManager _triggerManager = args.TriggerManager;
    private readonly ILogger _logger = args.Logger;


    public ISimulationDeviceId Id { get; } = args.Device.Id;
    public LifecycleState State => _stateMachine.State;


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.TransitionToRunningAsync(_taskManager.StartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.TransitionToRunningAsync(_taskManager.StopAsync, cancellationToken);
    
    public async ValueTask DisposeAsync()
    {
        if (!_disposeFlag.TryMarkDisposed()) return;

        await Safe.DisposeSafelyAsync(_taskManager, _logger);
        await Safe.DisposeSafelyAsync(_stateMachine, _logger);
    }


    public async ValueTask<IDisposable> ValueAsync<T>(DataAccessDescriptor<T> tagDescriptor, TriggerDescriptor triggerDescriptor, IValueGenerator<T> generator, CancellationToken cancellationToken = default)
        where T : notnull
    {
        if (tagDescriptor.Address is not ModbusProtocolAddress dataAddrss)
            throw new InvalidOperationException("不支持的标签");

        if (tagDescriptor.Codec is not IRequiredBits requiredBits)
            throw new InvalidOperationException("不支持的标签");

        var scheduler = await _triggerManager.GetSchedulerAsync(triggerDescriptor, cancellationToken);
        return scheduler.Subcribe(() =>
        {
            var value = generator.Next();
            _logger.LogTrace("设备({Id}) 已生成值({Value})", Id, value);

            OnValue(tagDescriptor, value);
        });
    }

    protected abstract void OnValue<T>(DataAccessDescriptor<T> tag, T value) where T : notnull;
}