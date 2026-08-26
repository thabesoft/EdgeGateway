using EdgeGateway.Modbus;
using System.Text.Json.Serialization;

namespace EdgeGateway.Options;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(ModbusDataAddressDto), typeDiscriminator: "Modbus")]
public abstract record class DataAddressDto;


/// <summary>
/// Modbus 地址
/// </summary>
public sealed record class ModbusDataAddressDto : DataAddressDto
{
    public required byte SlaveId { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter<ModbusRegion>))]
    public required ModbusRegion Region { get; init; }

    public required ushort StartAddress { get; init; }

    public required byte Quantity { get; init; }
}
