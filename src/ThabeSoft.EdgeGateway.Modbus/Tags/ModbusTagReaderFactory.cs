using ThabeSoft.EdgeGateway.Protocols;
using NModbus;
using ThabeSoft.EdgeGateway.IO;

namespace ThabeSoft.EdgeGateway.Tags;

internal sealed class ModbusTagReaderFactory(IModbusFactory modbusFactory) : ITagReaderFactory
{
    public bool CanCreate(IProtocolDescriptor descriptor)
    {
        return descriptor is ModbusProtocolDescriptor;
    }

    public ITagReader Create(IProtocolDescriptor descriptor, IByteStream byteStream)
    {
        if (descriptor is not ModbusProtocolDescriptor modbus)
            throw new ArgumentException("描述不是 Modbus RTU。", nameof(descriptor));

        var modbusMaster = modbusFactory.Create(modbus.FrameFormat, byteStream);
        return new ModbusTagReader(modbusMaster);
    }
}
