using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Configuration.Nodes;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Modbus;


internal sealed class ModbusProtocolDescriptorConfigMapper : IConfigMapper<ProtocolDescriptor>
{
    public bool CanMap(IConfigNode root)
    {
        if (!Enum.TryParse<ProtocolType>(root.GetStringByPath("Type"), true, out var enumValue))
            return false;

        return enumValue == ProtocolType.Modbus;
    }

    public ProtocolDescriptor Map(IConfigNode root)
    {
        var frameFormat = root.GetStringByPath("FrameFormat").Required("缺少帧类型")
                .ToEnum<ModbusFrameFormat>();

        return ModbusProtocolDescriptor.Create(frameFormat);
    }
}