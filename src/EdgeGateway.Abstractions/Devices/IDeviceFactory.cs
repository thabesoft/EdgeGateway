using EdgeGateway.Ports;
using EdgeGateway.Protocols;
using FluentResults;

namespace EdgeGateway.Devices;


/// <summary>
/// 设备构建工厂接口，定义了创建设备实例的方法
/// </summary>
public interface IDeviceFactory
{
    bool CanHandle(IPort port, IDeviceOptions config);
    Result<IDevice> Create(IPort port, IDeviceOptions config);
}

/// <summary>
/// 设备选项
/// </summary>
public interface IDeviceOptions
{
    DeviceId DeviceId { get; }
    ProtocolType Protocol { get; }
    PortAddress PortAddress { get; }
}