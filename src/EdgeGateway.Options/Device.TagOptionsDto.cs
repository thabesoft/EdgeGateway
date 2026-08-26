using EdgeGateway.Options.Serialization.Converters;
using System.Text.Json.Serialization;

namespace EdgeGateway.Options;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(NumberTagOptionsDto), typeDiscriminator: "Number")]
public abstract record class TagOptionsDto
{
    [JsonConverter(typeof(CleanStringConverter))]
    public required string Name { get; init; }

    [JsonConverter(typeof(CleanStringConverter))]
    public required string ByteOrder { get; init; }

    public required DataAddressDto Address { get; init; }
}


public sealed record class NumberTagOptionsDto : TagOptionsDto
{
    [JsonConverter(typeof(CleanStringConverter))]
    public required string DataType { get; init; }

    public string? Unit { get; init; }
    public double Scale { get; init; } = 1;
    public double Offset { get; init; } = 0;

    public required double AlarmLow { get; init; }
    public required double AlarmHigh { get; init; }
}