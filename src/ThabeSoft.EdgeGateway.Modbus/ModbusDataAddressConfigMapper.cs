using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Modbus;


internal sealed class ModbusDataAddressConfigMapper : IConfigMapper<DataAddress>
{
    public bool CanMap(IConfigNode root)
    {
        return GetProtocolType(root) == ProtocolType.Modbus;
    }

    public DataAddress Map(IConfigNode root)
    {
        var kind = root.GetStringValue("Kind").Required("缺少数据区域类型")
                .ToEnum<ModbusDataKind>();
        var slaveId = root.GetStringValue("SlaveId").Required("缺少从站Id")
                .ToByte();
        var address = root.GetStringValue("Address").Required("缺少数据起始地址")
                .ToUInt16();
        var quantity = root.GetStringValue("Quantity").Required("缺少数据数量")
                .ToUInt16();

        return kind switch
        {
            ModbusDataKind.Coil => ModbusDataAddress.Coil(slaveId, address, quantity),
            ModbusDataKind.DiscreteInput => ModbusDataAddress.DiscreteInput(slaveId, address, quantity),
            ModbusDataKind.HoldingRegister => ModbusDataAddress.HoldingRegister(slaveId, address, quantity),
            ModbusDataKind.InputRegister => ModbusDataAddress.InputRegister(slaveId, address, quantity),
            _ => throw new InvalidOperationException($"无法处理的数据区域: {kind}")
        };
    }


    private static ProtocolType? GetProtocolType(IConfigNode root)
    {
        var typeString = root.Parent?.Parent?["Protocol:Type"]?.AsString();

        if (!Enum.TryParse<ProtocolType>(typeString, true, out var type))
            return default;

        return type;
    }
}