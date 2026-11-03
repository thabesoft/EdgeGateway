using ThabeSoft.EdgeGateway.Channels;

namespace ThabeSoft.EdgeGateway.Options;


public sealed class GatewayOptionsBuilder : IGatewayOptionsBuilder
{
    private readonly Dictionary<ChannelAddress, PortOptions> _ports = [];

    public IGatewayOptionsBuilder AddPort(Action<IPortOptionsBuilder> buildAction)
    {
        var builder = new PortConfigBuilder();
        buildAction.Invoke(builder);
        var options = builder.Build();

        _ports[options.Descriptor.ChannelDescriptor.Address] = options;
        return this;
    }


    public GatewayOptions Build()
    {
        return new()
        {
            Ports = [.. _ports.Values]
        };
    }
}