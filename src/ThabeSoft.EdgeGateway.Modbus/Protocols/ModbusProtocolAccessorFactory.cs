using NModbus;
using ThabeSoft.EdgeGateway.Transport.Byte;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Protocols;


internal sealed class ModbusProtocolAccessorFactory(IModbusFactory modbusFactory) : IProtocolAccessorFactory
{
    public bool CanCreate(IProtocolDescriptor descriptor)
    {
        return descriptor is ModbusProtocolDescriptor;
    }

    public IProtocolAccessor Create(IProtocolDescriptor descriptor, IByteDuplex byteStream)
    {
        if (descriptor is not ModbusProtocolDescriptor modbus)
            throw new ArgumentException("描述不是 Modbus RTU。", nameof(descriptor));

        var modbusMaster = modbusFactory.Create(modbus.FrameFormat, byteStream);
        return new ModbusProtocolAccessor(modbusMaster);
    }
}
