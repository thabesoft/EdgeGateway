namespace ThabeSoft.EdgeGateway.Protocols;


public sealed class NotSupportedProtocolException(ProtocolDescriptor protocol) :
    NotSupportedException($"不支持的协议 [{protocol.GetType()}]: {protocol}")
{
    public ProtocolDescriptor Protocol { get; } = protocol ?? throw new ArgumentNullException(nameof(protocol));
}