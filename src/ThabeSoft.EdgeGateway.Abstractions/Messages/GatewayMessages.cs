using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Lifecycles;
using ThabeSoft.PubSub;

namespace ThabeSoft.EdgeGateway.Messages;


public readonly record struct GatewayStateChangedMessage(LifecycleState Old, LifecycleState New) : IMessage;

public readonly record struct PortStateChangedMessage(PortId PortId, LifecycleState Old, LifecycleState New) : IMessage;
public readonly record struct PortAddedMessage(PortId PortId) : IMessage;
public readonly record struct PortRemovedMessage(PortId PortId) : IMessage;