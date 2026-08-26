using EdgeGateway.Options;
using EdgeGateway.Ports;
using EdgeGateway.Protocols;
using FluentResults;

namespace EdgeGateway.Devices.Modbus;


public sealed class ModbusDeviceFactory : IDeviceFactory
{
    private ModbusDeviceFactory() { }
    public static ModbusDeviceFactory Instance { get; } = new();


    public bool CanHandle(IPort port, DeviceOptions config)
    {
        return config.Protocol == ProtocolType.ModbusRtu || config.Protocol == ProtocolType.ModbusAscii || config.Protocol == ProtocolType.ModbusIp;
    }

    public Result<IDevice> Create(IPort port, DeviceOptions config)
    {
        if (config.Protocol == ProtocolType.ModbusRtu)
        {
            return ModbusDevice.CreateRtu(port, config.DeviceId);
        }

        if (config.Protocol == ProtocolType.ModbusAscii)
        {
            return ModbusDevice.CreateAscii(port, config.DeviceId);
        }

        if (config.Protocol == ProtocolType.ModbusIp)
        {
            return ModbusDevice.CreateIp(port, config.DeviceId);
        }

        return Result.Fail("不支持的协议类型");
    }
}