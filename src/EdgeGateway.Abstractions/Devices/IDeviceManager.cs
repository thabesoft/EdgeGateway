namespace EdgeGateway.Devices;


/// <summary>
/// 设备管理器
/// </summary>
public interface IDeviceManager
{
    /// <summary>
    /// 所有已添加的设备
    /// </summary>
    IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }

    /// <summary>
    /// 根据配置创建一个通讯端点
    /// </summary>
    IDevice Add(DeviceDescriptor descriptor);
    /// <summary>
    /// 根据通讯端点唯一Id断开并移除
    /// </summary>
    ValueTask RemoveAsync(DeviceId id, CancellationToken cancellationToken = default);
}