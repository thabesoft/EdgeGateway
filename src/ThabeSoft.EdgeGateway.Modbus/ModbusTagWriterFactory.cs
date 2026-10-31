using ThabeSoft.EdgeGateway.Byte;
using ThabeSoft.EdgeGateway.Protocol;
using ThabeSoft.EdgeGateway.Tag;
using NModbus;

namespace ThabeSoft.EdgeGateway.Modbus;

internal sealed class ModbusTagWriterFactory(IModbusFactory modbusFactory) : ITagWriterFactory
{
    public bool CanCreate(IProtocolDescriptor descriptor)
    {
        return descriptor is ModbusProtocolDescriptor;
    }

    public ITagWriter Create(IProtocolDescriptor descriptor, IByteStream byteStream)
    {
        if (descriptor is not ModbusProtocolDescriptor modbus)
            throw new ArgumentException("描述不是 Modbus RTU。", nameof(descriptor));

        var modbusMaster = modbusFactory.Create(modbus.FrameFormat, byteStream);
        return new ModbusTagWriter(modbusMaster);
    }
}