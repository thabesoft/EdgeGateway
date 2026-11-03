using ThabeSoft.EdgeGateway.Channels;

namespace ThabeSoft.EdgeGateway.App.Messages;

public sealed record class PortRemovedMessage(ChannelAddress ChannelAddress);