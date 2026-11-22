using ThabeSoft.Configuration.Nodes;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Configuration.Mapping;

namespace ThabeSoft.Protocols.Modbus;


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