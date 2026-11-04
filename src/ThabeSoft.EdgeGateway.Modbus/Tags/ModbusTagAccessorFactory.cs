using ThabeSoft.EdgeGateway.Protocols;
using NModbus;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Tags;


internal sealed class ModbusTagAccessorFactory(IModbusFactory modbusFactory) : ITagAccessorFactory
{
    public bool CanCreate(IProtocolDescriptor descriptor)
    {
        return descriptor is ModbusProtocolDescriptor;
    }

    public ITagAccessor Create(IProtocolDescriptor descriptor, IByteDuplex byteStream)
    {
        if (descriptor is not ModbusProtocolDescriptor modbus)
            throw new ArgumentException("描述不是 Modbus RTU。", nameof(descriptor));

        var modbusMaster = modbusFactory.Create(modbus.FrameFormat, byteStream);
        return new ModbusTagAccessor(modbusMaster);
    }
}
