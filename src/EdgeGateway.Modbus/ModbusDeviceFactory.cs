using EdgeGateway.Devices;
using EdgeGateway.Ports;
using EdgeGateway.Protocols;
using FluentResults;

namespace EdgeGateway.Modbus;


/// <summary>
/// SerialPort/Rtu
/// </summary>
internal sealed class ModbusRtuDeviceFactory : IDeviceFactory
{
    public bool CanHandle(IPort port, IDeviceOptions options)
        => port.Address.PortType == PortType.SerialPort && options.Protocol == ProtocolType.ModbusRtu;

    public Result<IDevice> Create(IPort port, IDeviceOptions options)
        => ModbusDevice.CreateRtu(port, options.DeviceId);
}

/// <summary>
/// SerialPort/Ascii
/// </summary>
internal sealed class ModbusAsciiDeviceFactory : IDeviceFactory
{
    public bool CanHandle(IPort port, IDeviceOptions options)
        => port.Address.PortType == PortType.SerialPort && options.Protocol == ProtocolType.ModbusAscii;

    public Result<IDevice> Create(IPort port, IDeviceOptions options)
        => ModbusDevice.CreateRtu(port, options.DeviceId);
}

/// <summary>
/// Ip/Tcp
/// </summary>
internal sealed class ModbusIpDeviceFactory : IDeviceFactory
{
    public bool CanHandle(IPort port, IDeviceOptions options)
        => (port.Address.PortType == PortType.Tcp || port.Address.PortType == PortType.Udp) && options.Protocol == ProtocolType.ModbusIp;

    public Result<IDevice> Create(IPort port, IDeviceOptions options)
        => ModbusDevice.CreateIp(port, options.DeviceId);
}