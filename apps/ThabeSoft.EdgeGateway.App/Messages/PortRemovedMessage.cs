using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.App.Messages;

public sealed record class PortRemovedMessage(PortId PortId);