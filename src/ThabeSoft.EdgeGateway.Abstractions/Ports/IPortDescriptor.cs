using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


public interface IPortDescriptor
{
    ITransportDescriptor Transport { get; }
}

public sealed record class PortDescriptor : IPortDescriptor
{
    public required ITransportDescriptor Transport { get; init; }


    private PortDescriptor() { }
    public static PortDescriptor Create(ITransportDescriptor channelDescriptor)
    {
        ArgumentNullException.ThrowIfNull(channelDescriptor);

        return new()
        {
            Transport = channelDescriptor
        };
    }
}