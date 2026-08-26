using EdgeGateway.Options;
using EdgeGateway.Ports;
using FluentResults;

namespace EdgeGateway.Devices;


/// <summary>
/// 设备构建工厂接口，定义了创建设备实例的方法
/// </summary>
public interface IDeviceFactory
{
    bool CanHandle(IPort port, DeviceOptions config);
    Result<IDevice> Create(IPort port, DeviceOptions config);
}