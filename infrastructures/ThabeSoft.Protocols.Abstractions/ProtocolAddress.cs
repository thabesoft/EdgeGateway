using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.Protocols;


public abstract record class ProtocolAddress
{
    public abstract ProtocolType ProtocolType { get; }
}