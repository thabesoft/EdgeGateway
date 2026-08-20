using EdgeGateway.Ports;
using FluentResults;

namespace EdgeGateway.Devices;


/// <summary>
/// 设备构建工厂接口，定义了创建设备实例的方法
/// </summary>
public interface IDeviceFactory
{
    bool CanHandle(IPort port, DeviceConfig config);
    Result<IDevice> Create(IPort port, DeviceConfig config);
}