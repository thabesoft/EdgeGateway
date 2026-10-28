using EdgeGateway.Byte.Codecs;
using EdgeGateway.Device;
using EdgeGateway.Device.Polling;
using EdgeGateway.Modbus.Tag;
using EdgeGateway.Tag;
using NModbus;
using System.Buffers.Binary;

namespace EdgeGateway.Modbus.Device;


internal sealed class ModbusDevice(ModbusTagProtocol tagStream, IPollingScheduler pollingScheduler) : ITagDevice
{
    public DeviceId Id { get; init; } = DeviceId.Create();
    public ProtocolType ProtocolType => ProtocolType.Modbus;
    public bool IsEnabled { get; private set; }



    public ValueTask EnableAsync(CancellationToken ct = default)
    {
        if (IsEnabled) return ValueTask.CompletedTask;
        IsEnabled = true;

        pollingScheduler.StartAsync();
        return ValueTask.CompletedTask;
    }
    public ValueTask DisableAsync(CancellationToken ct = default)
    {
        if (!IsEnabled) return ValueTask.CompletedTask;
        IsEnabled = true;


        return ValueTask.CompletedTask;
    }


    public ValueTask<T> ReadAsync<T>(ITag<T> tag, CancellationToken cancellationToken = default)
        where T : notnull
    {
        ThrowIfDisable();
        return tagStream.ReadAsync(tag, cancellationToken);
    }
    public ValueTask WriteAsync<T>(ITag<T> tag, T value, CancellationToken cancellationToken = default)
        where T : notnull
    {
        ThrowIfDisable();
        return tagStream.WriteAsync(tag, value, cancellationToken);
    }



    private void ThrowIfDisable()
    {
        if (IsEnabled) return;
        throw new InvalidOperationException("设备已禁用");
    }
}
