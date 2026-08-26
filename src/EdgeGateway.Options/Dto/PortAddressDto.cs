namespace EdgeGateway.Dto;


/// <summary>
/// 端口地址
/// </summary>
public sealed record class PortAddressDto
{
    /// <summary>
    /// 类型
    /// </summary>
    public required string Type { get; init; }
    /// <summary>
    /// 地址
    /// </summary>
    public required string Location { get; init; }
}