namespace ThabeSoft.EdgeGateway.Transport;

public interface ITransportDescriptor
{
    ConnectionEndpoint Address { get; }
}