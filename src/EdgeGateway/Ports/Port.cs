using EdgeGateway.Bytes;
using EdgeGateway.Channel;
using EdgeGateway.Devices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EdgeGateway.Ports;

internal sealed class PortFactory(IPortManager portManager, IDeviceManager deviceManager, ILogger<PortFactory> logger) : IPortFactory
{
    public bool CanCreate(ChannelDescriptor descriptor)
    {
        throw new NotImplementedException();
    }

    public IPort Create(ChannelDescriptor descriptor)
    {
        return new Port(portManager, deviceManager, Options.Create(new PortOptions()), logger);
    }
}

/// <summary>
/// 端口句柄
/// </summary>
file sealed class Port(
        IPortManager portManager,
        IDeviceManager deviceManager,
        IChannel channel,
        IOptions<PortOptions> options
    ) : IPort
{
    public bool IsOpen => channel.IsOpen;
    public ChannelDescriptor Descriptor => throw new NotImplementedException();
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => deviceManager.Devices;


    public async ValueTask OpenAsync(CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(options.Value.OpenTimeout);

        try
        {
            await channel.OpenAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"端口 {Descriptor} 开启超时");
        }
    }
    public async ValueTask CloseAsync(CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(options.Value.CloseTimeout);

        try
        {
            await channel.CloseAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"端口 {Descriptor} 关闭超时");
        }
    }

    public IDevice AddDevice(DeviceDescriptor descriptor)
    {
        return deviceManager.Add(descriptor);
    }
    public async ValueTask RemoveDeviceAsync(DeviceId id, CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(options.Value.RemoveDeviceTimeout);

        try
        {
            await deviceManager.RemoveAsync(id, cts.Token);
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"端口 {Descriptor.Address} 移除设备 {id} 超时");
        }
    }


    public async ValueTask DisposeAsync()
    {
        await portManager.RemoveAsync(Descriptor.Address);
        await channel.DisposeAsync();
    }
}


public sealed class PortOptions
{
    /// <summary>
    /// 移除设备超时时长
    /// </summary>
    public TimeSpan RemoveDeviceTimeout { get; init; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// 开启超时时长
    /// </summary>
    public TimeSpan OpenTimeout { get; init; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// 关闭超时时长
    /// </summary>
    public TimeSpan CloseTimeout { get; init; } = TimeSpan.FromSeconds(5);
}