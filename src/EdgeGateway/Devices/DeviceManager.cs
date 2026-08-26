using EdgeGateway.Data;
using EdgeGateway.Ports;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace EdgeGateway.Devices;


internal sealed class DeviceManager(IEnumerable<IDeviceFactory> deviceFactories, ILogger<DeviceManager> logger) : IDeviceManager
{
    private readonly Dictionary<DeviceId, IDevice> _devices = [];
    public IReadOnlyDictionary<DeviceId, IDevice> Devices => _devices;


    public Result<IDevice> Add(IPort port, DeviceOptions config)
    {
        var createErrors = new List<IError>();

        foreach (var factory in deviceFactories.ToArray())
        {
            if (!factory.CanHandle(port, config)) continue;

            var createResult = factory.Create(port, config);
            if (createResult.IsFailed)
            {
                createErrors.Add(new Error($"{factory.GetType()}构建失败").CausedBy(createResult.Errors));
                continue;
            }

            var device = createResult.Value;
            _devices[device.Id] = new DeviceHandle(this, device, logger);
            return Result.Ok(_devices[device.Id]);
        }

        return Result.Fail("没有工厂能处理该配置").WithErrors(createErrors);
    }

    public async ValueTask<Result> RemovAsync(DeviceId id, CancellationToken cancellationToken = default)
    {
        if (!_devices.TryGetValue(id, out var device))
            return Result.Ok();

        var disconnectResult = await device.DisconnectAsync(cancellationToken);
        if (disconnectResult.IsFailed)
            return Result.Fail("设备无法移除").WithErrors(disconnectResult.Errors);

        _devices.Remove(device.Id);
        return Result.Ok();
    }
}


/// <summary>
/// 设备句柄
/// </summary>
/// <param name="deviceManager">设备管理器</param>
/// <param name="device">设备实例</param>
file class DeviceHandle(IDeviceManager deviceManager, IDevice device, ILogger<DeviceManager> logger) : IDevice
{
    public async ValueTask DisposeAsync()
    {
        var result = await deviceManager.RemovAsync(device.Id);
        if (result.IsFailed) logger.LogError("释放{msg}", string.Join(Environment.NewLine, result.Errors.Select(x => x.Message)));
        //TODO: 需要设计重试删除机制


        await device.DisposeAsync();
    }



    #region --转发--

    public DeviceId Id => device.Id;
    public PortAddress PortAddress => device.PortAddress;

    public ValueTask<Result> ConnectAsync(CancellationToken cancellationToken = default)
        => device.ConnectAsync(cancellationToken);
    public ValueTask<Result> DisconnectAsync(CancellationToken cancellationToken = default)
        => device.DisconnectAsync(cancellationToken);

    public ValueTask<Result<T>> ReadAsync<T>(DataDescriptor<T> descriptor, CancellationToken cancellationToken = default)
        => device.ReadAsync(descriptor, cancellationToken);
    public ValueTask<Result> WriteAsync<T>(DataDescriptor<T> descriptor, T value, CancellationToken cancellationToken = default)
        => device.WriteAsync(descriptor, value, cancellationToken);

    #endregion
}