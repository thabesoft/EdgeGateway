using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Tags;


public abstract record class DataAddress
{
    public abstract ProtocolType ProtocolType { get; }
}