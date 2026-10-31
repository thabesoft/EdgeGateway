using Microsoft.Extensions.Hosting;
using NModbus;
using SharpHook;

namespace EdgeGateway.Services.ModbusSlave;


public sealed class MousePositionUpdater : IHostedService, IDisposable
{
    private readonly EventLoopGlobalHook _eventLoopGlobalHook = new();

    public MousePositionUpdater(ISlaveDataStore dataStore)
    {
        _eventLoopGlobalHook.MouseMoved += (sender, e) =>
            dataStore.HoldingRegisters.WritePoints(0, [(ushort)e.Data.X, (ushort)e.Data.Y]);
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        return _eventLoopGlobalHook.RunAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _eventLoopGlobalHook.Stop();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _eventLoopGlobalHook.Dispose();
    }
}