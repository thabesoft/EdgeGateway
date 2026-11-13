namespace ThabeSoft.EdgeGateway.Transport;


public interface ITransportDescriptor : IEquatable<ITransportDescriptor>
{
    ConnectionEndpoint Endpoint { get; }
}