using NModbus;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.Simulator.ValueGenerators;

namespace ThabeSoft.Simulator;


public interface IModbusSimulator : IModbusSimulatorData, ILifecycle, IAsyncDisposable
{
    byte UnitId { get; }
    IDisposable AddValueGenerato<T>(ITrigger trigger, IValueGenerator<T> generator, Action<IModbusSimulatorData, T> update)
        where T : notnull;
}


internal sealed class ModbusSimulator(byte unitId, ISlaveDataStore slaveDataStore) : LifecycleStateMachine, IModbusSimulator
{
    private readonly TaskManager _taskManager = new();

    public byte UnitId => unitId;


    public IDisposable AddValueGenerato<T>(ITrigger trigger, IValueGenerator<T> generator, Action<IModbusSimulatorData, T> update)
        where T : notnull
    {
        return _taskManager.AddTask(async ct =>
        {
            while (!ct.IsCancellationRequested)
            {
                if (!await trigger.WaitForNextTickAsync(ct))
                    return;

                var value = generator.Next();
                update.Invoke(this, value);
            }
        });
    }

    public void SetCoils(ushort address, params bool[] values)
    {
        slaveDataStore.CoilInputs.WritePoints(address, values);
    }
    public void SetHoldingRegisters(ushort address, params ushort[] values)
    {
        slaveDataStore.HoldingRegisters.WritePoints(address, values);
    }

    protected override ValueTask OnStartAsync(CancellationToken cancellationToken) =>
        _taskManager.StartAsync(cancellationToken);
    protected override ValueTask OnStopAsync(CancellationToken cancellationToken) =>
        _taskManager.StopAsync(cancellationToken);
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
        try { await Task.WhenAll(tasks!).WaitAsync(cancellationToken); }
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
            await _startSignal.Task.WaitAsync(lifetimeToken);   // 等启动
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