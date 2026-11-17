using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Transports;


public abstract record class TransportDescriptor
{
    public abstract ConnectionEndpoint Endpoint { get; }
    public RetryDescriptor Retry { get; init => field = value.Required(); } = Transports.Retry.Default;
}