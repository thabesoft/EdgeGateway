namespace ThabeSoft.EdgeGateway.Configs;

public sealed class GatewayConfigBuilder
{
    private readonly List<PortConfig> _ports = [];


    public GatewayConfigBuilder AddPort(PortConfig config)
    {
        if (_ports.Contains(config)) return this;

        _ports.Add(config);
        return this;
    }

    public GatewayConfigBuilder AddPort(Action<PortConfigBuilder> buildAction)
    {
        var builder = new PortConfigBuilder();
        buildAction.Invoke(builder);

        return AddPort(builder.Build());
    }


    public GatewayConfig Build()
    {
        return new()
        {
            Ports = _ports.AsReadOnly()
        };
    }
}