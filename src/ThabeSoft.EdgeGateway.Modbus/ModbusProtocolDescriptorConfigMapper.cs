using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Modbus;


internal sealed class ModbusProtocolDescriptorConfigMapper : IConfigMapper<ProtocolDescriptor>
{
    public bool CanMap(IConfigNode root)
    {
        if (!Enum.TryParse<TransportType>(root.GetValue("Type"), true, out var enumValue))
            return false;

        return enumValue == TransportType.Serial;
    }

    public ProtocolDescriptor Map(IConfigNode root)
    {
        var frameFormat = root.GetValue("FrameFormat").Required("缺少帧类型")
                .ToEnum<ModbusFrameFormat>();

        return ModbusProtocolDescriptor.Creaete(frameFormat);
    }
}