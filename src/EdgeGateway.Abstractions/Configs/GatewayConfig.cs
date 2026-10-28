namespace EdgeGateway.Configs;


public sealed record class GatewayConfig
{
    public required IReadOnlyList<PortConfig> Ports { get; init; }


    internal GatewayConfig() { }
    public static GatewayConfigBuilder Builder() => new();
}