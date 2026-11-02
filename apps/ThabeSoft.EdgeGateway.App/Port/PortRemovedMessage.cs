using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.App.Port;

public sealed record class PortRemovedMessage(PortId PortId);