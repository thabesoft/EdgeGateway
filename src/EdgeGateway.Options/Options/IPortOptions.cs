using EdgeGateway.Ports;

namespace EdgeGateway.Options;

public interface IPortOptions
{
    PortAddress Address { get; }
}