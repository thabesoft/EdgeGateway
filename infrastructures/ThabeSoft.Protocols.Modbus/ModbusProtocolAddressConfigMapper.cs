using ThabeSoft.Configuration;
using ThabeSoft.Configuration.Nodes;
using ThabeSoft.EdgeGateway.Primitives;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.Protocols.Modbus;


internal sealed class ModbusProtocolAddressConfigMapper : IConfigMapper<ProtocolAddress>
{
    public bool CanMap(IConfigNode root)
    {
        return GetProtocolType(root) == ProtocolType.Modbus;
    }

    public ProtocolAddress Map(IConfigNode root)
    {
        var kind = root.GetStringByPath("Kind").Required("缺少数据区域类型")
                .ToEnum<ModbusDataKind>();
        var slaveId = root.GetStringByPath("SlaveId").Required("缺少从站Id")
                .ToByte();
        var address = root.GetStringByPath("Address").Required("缺少数据起始地址")
                .ToUInt16();
        var quantity = root.GetStringByPath("Quantity").Required("缺少数据数量")
                .ToUInt16();

        return kind switch
        {
            ModbusDataKind.Coil => ModbusProtocolAddress.Coil(slaveId, address, quantity),
            ModbusDataKind.DiscreteInput => ModbusProtocolAddress.DiscreteInput(slaveId, address, quantity),
            ModbusDataKind.HoldingRegister => ModbusProtocolAddress.HoldingRegister(slaveId, address, quantity),
            ModbusDataKind.InputRegister => ModbusProtocolAddress.InputRegister(slaveId, address, quantity),
            _ => throw new InvalidOperationException($"无法处理的数据区域: {kind}")
        };
    }


    private static ProtocolType? GetProtocolType(IConfigNode root)
    {
        var typeString = root.GetByPath("......Protocol.Type")?.AsString();

        if (!Enum.TryParse<ProtocolType>(typeString, true, out var type))
            return default;

        return type;
    }
}