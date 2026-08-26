using EdgeGateway.Devices;

namespace EdgeGateway.Dto;


/// <summary>
/// 网关选项
/// </summary>
public sealed record class GatewayOptionsDto
{
    public required IReadOnlyList<PortOptionsDto> Ports { get; init; }
    public required IReadOnlyList<DeviceOptionsDto> Devices { get; init; }
}