using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


public readonly record struct GatewayPortAddedEvent(IPort Port) : IEvent;
public readonly record struct GatewayPortRemovedEvent(ConnectionEndpoint Endpoint) : IEvent;
public readonly record struct GatewayPortStateChangedEvent(ConnectionEndpoint Endpoint, LifecycleState OldState, LifecycleState NewState) : IEvent;