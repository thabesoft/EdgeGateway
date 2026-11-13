namespace ThabeSoft.EdgeGateway.Protocols;

public interface IProtocolDescriptor : IEquatable<IProtocolDescriptor>
{
    ProtocolType Type { get; }
}