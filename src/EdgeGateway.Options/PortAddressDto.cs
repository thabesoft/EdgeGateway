using EdgeGateway.Options.Serialization.Converters;
using EdgeGateway.Ports;
using System.Text.Json.Serialization;

namespace EdgeGateway.Options;


/// <summary>
/// 端口地址
/// </summary>
public sealed record class PortAddressDto
{
    /// <summary>
    /// 类型
    /// </summary>
    [JsonConverter(typeof(PortTypeConverter))]
    public required TransportProtocol Type { get; init; }

    /// <summary>
    /// 地址
    /// </summary>
    [JsonConverter(typeof(CleanStringConverter))]
    public required string Location { get; init; }
}