using NModbus;
using NModbus.IO;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.IO;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transports.Byte;

namespace ThabeSoft.EdgeGateway.Modbus.Protocols;


internal sealed class ModbusProtocolAccessorResolver(
        IModbusFactory modbusFactory,
        ICodecFactory codecFactory
    ) : IProtocolAccessorResolver
{
    public bool CanResolve(ProtocolDescriptor descriptor)
    {
        return descriptor is ModbusProtocolDescriptor;
    }

    public IProtocolAccessor Resolve(ProtocolDescriptor descriptor, IByteDuplex byteStream)
    {
        if (descriptor is not ModbusProtocolDescriptor modbus)
            throw new ArgumentException("描述不是 Modbus RTU。", nameof(descriptor));

        var modbusMaster = Create(modbus.FrameFormat, byteStream);
        return new ModbusProtocolAccessor(modbusMaster, codecFactory);
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
