using System.Transactions;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.EdgeGateway.Events;

public interface IEvent;


public readonly record struct GatewayStateChanged(LifecycleState Old, LifecycleState New);


public readonly record struct PortStateChanged(PortId PortId, LifecycleState Old, LifecycleState New) : IEvent;
public readonly record struct PortAddedEvent(PortId PortId) : IEvent;
public readonly record struct PortRemovedEvent(PortId PortId) : IEvent;


public readonly record struct DeviceStateChanged(PortId PortId, DeviceId DeviceId, LifecycleState Old, LifecycleState New) : IEvent;
public readonly record struct DeviceAddedEvent(PortId PortId, DeviceId DeviceId) : IEvent;
public readonly record struct DeviceRemovedEvent(PortId PortId, DeviceId DeviceId) : IEvent;


public readonly record struct SubscriptionStateChanged(PortId PortId, DeviceId DeviceId, SubscriptionId SubscriptionId, LifecycleState Old, LifecycleState New) : IEvent;
public readonly record struct SubscriptionAddedEvent(PortId PortId, DeviceId DeviceId, SubscriptionId SubscriptionId) : IEvent;
public readonly record struct SubscriptionRemovedEvent(PortId PortId, DeviceId DeviceId, SubscriptionId SubscriptionId) : IEvent;