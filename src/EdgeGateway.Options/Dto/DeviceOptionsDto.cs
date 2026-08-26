using EdgeGateway.Dto;

namespace EdgeGateway.Devices;


/// <summary>
/// 设备选项
/// </summary>
public sealed record class DeviceOptionsDto
{
    public required string Id { get; init; }
    public required string Protocol { get; init; }
    public required PortAddressDto PortAddress { get; init; }
}
