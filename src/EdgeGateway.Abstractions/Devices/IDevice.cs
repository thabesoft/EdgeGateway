namespace EdgeGateway.Devices;


/// <summary>
///  表示一个具有唯一标识和寻址位置的通信设备，具备可启用/禁用的生命周期
/// </summary>
public interface IDevice : IAsyncDisposable
{
    DeviceId Id { get; }
    DeviceDescriptor Descriptor { get; }
    bool IsEnable { get; }


    ValueTask EnableAsync(CancellationToken cancellationToken = default);
    ValueTask DisableAsync(CancellationToken cancellationToken = default);
}