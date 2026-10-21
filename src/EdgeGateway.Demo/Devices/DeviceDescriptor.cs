using EdgeGateway.Channels;

namespace EdgeGateway.Devices;

public interface IDeviceDescriptor
{
    ProtocolType Protocol { get; }
    ConnectionType Connection { get; }
}

public sealed record class DeviceDescriptor : IDeviceDescriptor
{
    public required ConnectionType Connection { get; init; }
    public required ProtocolType Protocol { get; init; }
}

public sealed record class ModbusDeviceDescriptor : IDeviceDescriptor
{
    public ProtocolType Protocol { get; } = ProtocolType.Modbus;
    public required ConnectionType Connection { get; init; }
    public required ModbusFrameFormat FrameFormat { get; init; }

    private ModbusDeviceDescriptor() { }



    public static ModbusDeviceDescriptor Rtu { get; } = new()
    {
        Connection = ConnectionType.Serial,
        FrameFormat = ModbusFrameFormat.Rtu
    };
    public static ModbusDeviceDescriptor Ascii { get; } = new()
    {
        Connection = ConnectionType.Serial,
        FrameFormat = ModbusFrameFormat.Ascii
    };
    public static ModbusDeviceDescriptor Tcp { get; } = new()
    {
        Connection = ConnectionType.Tcp,
        FrameFormat = ModbusFrameFormat.Ip
    };
    public static ModbusDeviceDescriptor Udp { get; } = new()
    {
        Connection = ConnectionType.Udp,
        FrameFormat = ModbusFrameFormat.Ip
    };
}


public enum ModbusFrameFormat { Rtu, Ascii, Ip }