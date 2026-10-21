using EdgeGateway.Devices;

namespace EdgeGateway.Ports;


/// <summary>
/// 表示一个可被打开和关闭的通信通道, 可以通过该通道与多个设备进行通信
/// </summary>
public interface IPort : IAsyncDisposable
{
    bool IsOpen { get; }
    IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }


    ValueTask OpenAsync(CancellationToken cancellationToken = default);
    ValueTask CloseAsync(CancellationToken cancellationToken = default);


    IDevice AddDevice(DeviceDescriptor config);
    ValueTask RemoveDeviceAsync(DeviceId id, CancellationToken cancellationToken = default);
}