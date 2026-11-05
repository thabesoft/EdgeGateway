using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Configuration;


internal sealed class GatewayOptionsBuilder : IGatewayOptionsConfigurator
{
    private bool _autoLaunch = true;
    private readonly Dictionary<ConnectionEndpoint, PortOptions> _ports = [];


    public IGatewayOptionsConfigurator WithAutoLaunch(bool value)
    {
        _autoLaunch = value;
        return this;
    }

    public IGatewayOptionsConfigurator AddPort(Action<IPortOptionsConfigurator> buildAction)
    {
        var builder = new PortConfigBuilder();
        buildAction.Invoke(builder);
        var options = builder.Build();

        _ports[options.Transport.Address] = options;
        return this;
    }

    public GatewayOptions Build()
    {
        return new()
        {
            AutoLaunch = _autoLaunch,
            Ports = [.. _ports.Values]
        };
    }
}

internal sealed record class GatewayOptions
{
    public required bool AutoLaunch { get; init; }
    public required IReadOnlyList<PortOptions> Ports { get; init; }
}