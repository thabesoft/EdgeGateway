using EdgeGateway.Devices;
using EdgeGateway.Options;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace EdgeGateway.Ports;


public interface IPortHandle : IPort
{
    IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }


    Result<IDevice> AddDevice(DeviceOptions config);
    ValueTask<Result> RemoveDeviceAsync(DeviceId id, CancellationToken cancellationToken = default);
}


/// <summary>
/// 端口句柄
/// </summary>
public sealed class PortHandle(IPortManager portManager, IDeviceManager deviceManager, IPort port, ILogger logger) : IPortHandle
{
    #region --生命周期--

    public async ValueTask<Result> OpenAsync(CancellationToken cancellationToken = default)
    {
        var openResult = await port.OpenAsync(cancellationToken);
        if (openResult.IsFailed) return openResult;

        var connectErrors = new List<IError>();

        foreach (var device in deviceManager.Devices.Values.ToArray())
        {
            var connectResult = await device.ConnectAsync(cancellationToken);
            if (connectResult.IsSuccess) continue;

            connectErrors.Add(new Error($"设备 {device.Id} 连接失败").CausedBy(connectResult.Errors));
        }

        if (connectErrors.Count == 0) return Result.Ok();
        return Result.Fail("端口开启成功, 但部分设备连接失败").WithErrors(connectErrors);
    }
    public async ValueTask<Result> CloseAsync(CancellationToken cancellationToken = default)
    {
        var closeResult = await port.CloseAsync(cancellationToken);
        if (closeResult.IsFailed) return closeResult;

        var disconnectErrors = new List<IError>();

        foreach (var device in deviceManager.Devices.Values.ToArray())
        {
            var connectResult = await device.DisconnectAsync(cancellationToken);
            if (connectResult.IsSuccess) continue;

            disconnectErrors.Add(new Error($"设备 {device.Id} 断开失败").CausedBy(connectResult.Errors));
        }

        if (disconnectErrors.Count == 0) return Result.Ok();
        return Result.Fail("端口关闭成功，但部分设备断开失败").WithErrors(disconnectErrors);
    }
    public async ValueTask DisposeAsync()
    {
        //TOOD: 释放状态检测

        try
        {
            var removeResult = await portManager.RemovAsync(port.Address);
            if (removeResult.IsFailed)
            {
                logger.LogWarning("DisposeAsync: RemovePort 失败: {Error}", removeResult);
            }

            var closeResult = await CloseAsync();
            if (closeResult.IsFailed)
            {
                logger.LogWarning("DisposeAsync: CloseAsync 失败: {Error}", closeResult);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DisposeAsync: CloseAsync 发生异常");
        }
        finally
        {
            // 强制释放端口资源
            if (port is IAsyncDisposable asyncDisposable)
            {
                try
                {
                    await asyncDisposable.DisposeAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "DisposeAsync: 释放端口资源失败");
                }
            }
        }
    }

    #endregion

    #region --转发--

    public PortAddress Address => port.Address;
    public bool IsOpen => port.IsOpen;



    public ValueTask<Result> ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        => port.ReadExactlyAsync(buffer, cancellationToken);
    public ValueTask<Result> WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        => port.WriteAsync(data, cancellationToken);


    public IReadOnlyDictionary<DeviceId, IDevice> Devices
      => deviceManager.Devices;
    public Result<IDevice> AddDevice(DeviceOptions config)
        => deviceManager.Add(port, config);
    public ValueTask<Result> RemoveDeviceAsync(DeviceId id, CancellationToken cancellationToken = default)
        => deviceManager.RemovAsync(id, cancellationToken);


    #endregion
}