namespace ThabeSoft.EdgeGateway.Options;


public sealed record class GatewayOptions
{
    public required IReadOnlyList<PortOptions> Ports { get; init; }


    internal GatewayOptions() { }
    public static GatewayOptionsBuilder Builder() => new();
}