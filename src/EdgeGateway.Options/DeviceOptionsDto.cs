using EdgeGateway.Options.Serialization.Converters;
using EdgeGateway.Protocols;
using System.Text.Json.Serialization;

namespace EdgeGateway.Options;


/// <summary>
/// 设备选项
/// </summary>
public sealed record class DeviceOptionsDto
{
    [JsonConverter(typeof(CleanStringConverter))]
    public required string Id { get; init; }

    [JsonConverter(typeof(SpanParsableConverter))]
    public required ProtocolType Protocol { get; init; }

    public required PortAddressDto PortAddress { get; init; }

    public required IReadOnlyCollection<TagOptionsDto> Tags { get; init; }
}