using NModbus;
using NModbus.IO;
using ThabeSoft.EdgeGateway.IO;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Protocols;


internal sealed class ModbusProtocolAccessorFactory(IModbusFactory modbusFactory) : IProtocolAccessorFactory
{
    public bool CanCreate(ProtocolDescriptor descriptor)
    {
        return descriptor is ModbusProtocolDescriptor;
    }

    public IProtocolAccessor Create(ProtocolDescriptor descriptor, IByteDuplex byteStream)
    {
        if (descriptor is not ModbusProtocolDescriptor modbus)
            throw new ArgumentException("描述不是 Modbus RTU。", nameof(descriptor));

        var modbusMaster = Create(modbus.FrameFormat, byteStream);
        return new ModbusProtocolAccessor(modbusMaster);
    }


    private IModbusMaster Create(ModbusFrameFormat modbusFrame, IStreamResource streamResource)
    {
        if (!Enum.IsDefined(modbusFrame))
            throw new ArgumentOutOfRangeException(nameof(modbusFrame));

        ArgumentNullException.ThrowIfNull(streamResource);


        return modbusFrame switch
        {
            ModbusFrameFormat.Rtu => modbusFactory.CreateRtuMaster(streamResource),
            ModbusFrameFormat.Ascii => modbusFactory.CreateAsciiMaster(streamResource),
            ModbusFrameFormat.Ip => modbusFactory.CreateIpMaster(streamResource),
            _ => throw new InvalidOperationException(),
        };
    }
    private IModbusMaster Create(ModbusFrameFormat modbusFrame, IByteDuplex byteStream)
    {
        ArgumentNullException.ThrowIfNull(byteStream);

        var streamResource = new NModbusStreamAdapter(byteStream);
        return Create(modbusFrame, streamResource);
    }
}
