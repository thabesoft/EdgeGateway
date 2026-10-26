using EdgeGateway.Byte;
using EdgeGateway.Device;
using NModbus;
using NModbus.IO;

namespace EdgeGateway.Modbus.Device;


internal sealed class ModbusDeviceFactory(IModbusFactory modbusFactory) : IDeviceFactory
{
    public bool CanCreate(IDeviceDescriptor descriptor)
    {
        return descriptor is ModbusDeviceDescriptor modbus;
    }
    public IDevice Create(IDeviceDescriptor descriptor, IByteStream byteChannel)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(byteChannel);

        if (descriptor is not ModbusDeviceDescriptor modbus)
            throw new ArgumentException("描述不是 Modbus RTU。", nameof(descriptor));

        if (!Enum.IsDefined(modbus.FrameFormat))
            throw new ArgumentOutOfRangeException(nameof(descriptor));

        var streamSource = new StreamResourceAdapter(byteChannel);
        var modbusMaster = modbus.FrameFormat switch
        {
            ModbusFrameFormat.Rtu => modbusFactory.CreateRtuMaster(streamSource),
            ModbusFrameFormat.Ascii => modbusFactory.CreateAsciiMaster(streamSource),
            ModbusFrameFormat.Ip => modbusFactory.CreateIpMaster(streamSource),
            _ => throw new InvalidOperationException(),
        };

        return new ModbusDevice(modbusMaster);
    }
}
