using EdgeGateway.Byte;
using EdgeGateway.Device;
using NModbus;
using NModbus.IO;

namespace EdgeGateway.Modbus.Device;


internal sealed class ModbusDeviceFactory(IModbusFactory modbusFactory) : IDeviceFactory
{
    public bool CanCreate(IDeviceDescriptor descriptor)
    {
        return descriptor is ModbusDeviceDescriptor;
    }

    public IDevice Create(IDeviceDescriptor descriptor, IByteStream byteChannel)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(byteChannel);

        if (descriptor is not ModbusDeviceDescriptor modbus)
            throw new ArgumentException("描述不是 Modbus RTU。", nameof(descriptor));

        if (!Enum.IsDefined(modbus.FrameFormat))
            throw new ArgumentOutOfRangeException(nameof(descriptor));


        // 构建
        var streamResource = new StreamResourceAdapter(byteChannel);
        var tagProtocol = CreateModbusTagProtocol(modbus.FrameFormat, streamResource);

        return new ModbusDevice(tagProtocol, null!);
    }


    private ModbusTagProtocol CreateModbusTagProtocol(ModbusFrameFormat modbusFrame, IStreamResource streamResource)
    {
        var modbusMaster = modbusFrame switch
        {
            ModbusFrameFormat.Rtu => modbusFactory.CreateRtuMaster(streamResource),
            ModbusFrameFormat.Ascii => modbusFactory.CreateAsciiMaster(streamResource),
            ModbusFrameFormat.Ip => modbusFactory.CreateIpMaster(streamResource),
            _ => throw new InvalidOperationException(),
        };

        return new ModbusTagProtocol(modbusMaster);
    }
}
