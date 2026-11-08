using NModbus;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Simulation.ValueGenerators;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;


internal sealed class ModbusSimulatedSlave(
        byte unitId,
        IModbusFactory modbusFactory,
        ITriggerFactorySelector triggerFactorySelector
    ) : IModbusSimulatedSlave
{
    private readonly LifecycleStateMachineA _stateMachine = Lifecycle.Normal();
    private readonly TaskManager _taskManager = new();
    private readonly ConcurrentDictionary<Guid, ITrigger> _triggers = [];


    public byte UnitId => unitId;
    public IModbusSlave Slave { get; } = modbusFactory.CreateSlave(unitId);
    public LifecycleState State => _stateMachine.State;




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
            while (!ct.IsCancellationRequested)
            {
                if (!await trigger.WaitForNextTickAsync(ct))
                    return;

                var value = generator.Next();

                if (dataAddrss.Kind == ModbusDataKind.HoldingRegister)
                {
                    var words = tagDescriptor.Codec.EncodeWords(value);
                    SetHoldingRegisters(dataAddrss.StartAddress, words);
                }
            }
        });
    }


    public void SetCoils(ushort address, params bool[] values)
    {
        Slave.DataStore.CoilInputs.WritePoints(address, values);
    }
    public void SetHoldingRegisters(ushort address, params ushort[] values)
    {
        Slave.DataStore.HoldingRegisters.WritePoints(address, values);
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(_taskManager, (state, ct) => state.StartAsync(ct), cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(_taskManager, (state, ct) => state.StopAsync(ct), cancellationToken);
    public ValueTask DisposeAsync() => _stateMachine.DisposeAsync(async () =>
    {
        foreach (var i in _triggers.Values)
            await i.DisposeAsync();

        await _taskManager.DisposeAsync();
    });
}



internal sealed class TaskManager
{
    private readonly ConcurrentDictionary<Guid, TaskEntry> _entries = [];
    private readonly object _lock = new();

    private CancellationTokenSource? _lifetimeCts;
    private TaskCompletionSource _startSignal = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private volatile bool _running;
    private volatile bool _disposed;


    public IDisposable AddTask(Func<CancellationToken, Task> task)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var id = Guid.NewGuid();
        var cts = new CancellationTokenSource();
        var entry = new TaskEntry(task, cts);

        _entries[id] = entry;

        if (_running)
        {
            // 已启动：立即跑
            entry.Task = RunAsync(entry, _lifetimeCts!.Token);
        }
        // 未启动：挂起，等 StartAsync 放行

        return new Releaser(() =>
        {
            _entries.TryRemove(id, out _);
            cts.Cancel();   // 取消这个任务
            cts.Dispose();
        });
    }


    public async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        lock (_lock)
        {
            if (_running) return;
            _running = true;
            _lifetimeCts ??= new CancellationTokenSource();
            _startSignal.TrySetResult();   // 放行“挂起”的任务
        }

        // 启动前添加的“挂起”任务，现在启动
        foreach (var entry in _entries.Values)
        {
            entry.Task ??= RunAsync(entry, _lifetimeCts.Token);
        }
    }

    public async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_running) return;
            _running = false;
            _lifetimeCts?.Cancel();
            _lifetimeCts?.Dispose();
            _lifetimeCts = null;
            _startSignal = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);   // 重置门闩
        }

        // 等所有任务退出
        var tasks = _entries.Values.Select(e => e.Task).Where(t => t is not null).ToArray();
        try { Task.WhenAll(tasks!).Wait(cancellationToken); }
        catch (OperationCanceledException) { }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        await StopAsync();
    }

    private async Task RunAsync(TaskEntry entry, CancellationToken lifetimeToken)
    {
        try
        {
            _startSignal.Task.Wait(lifetimeToken);   // 等启动
            await entry.TaskFunc(entry.Cts.Token);              // 跑任务
        }
        catch (OperationCanceledException) { }
        finally
        {
            entry.Cts.Dispose();
        }
    }


    private sealed class TaskEntry(Func<CancellationToken, Task> taskFunc, CancellationTokenSource cts)
    {
        public Func<CancellationToken, Task> TaskFunc { get; } = taskFunc;
        public CancellationTokenSource Cts { get; } = cts;
        public Task? Task { get; set; }
    }
    private sealed class Releaser(Action action) : IDisposable
    {
        private int _disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0) action();
        }
    }
}