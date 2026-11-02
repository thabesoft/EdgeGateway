namespace ThabeSoft.EdgeGateway.Channels;

public interface IChannelDescriptor
{
    ChannelId Id { get; }
    ConnectionType ConnectionType { get; }
}