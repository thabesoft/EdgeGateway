namespace EdgeGateway.Options.Loader;


public record class GatewaySourceOptions
{
    public required string Path { get; init; } = "gateway.json";
}