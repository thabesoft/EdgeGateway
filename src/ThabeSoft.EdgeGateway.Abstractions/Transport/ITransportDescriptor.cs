namespace ThabeSoft.EdgeGateway.Transport;

public interface ITransportDescriptor
{
    ConnectionEndpoint Endpoint { get; }
}