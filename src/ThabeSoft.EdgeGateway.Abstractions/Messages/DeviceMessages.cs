using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Lifecycles;
using ThabeSoft.PubSub;

namespace ThabeSoft.EdgeGateway.Messages;


public readonly record struct SubscriptionStateChangedMessage(PortId PortId, DeviceId DeviceId, SubscriptionId SubscriptionId, LifecycleState Old, LifecycleState New) : IMessage;
public readonly record struct SubscriptionAddedMessage(PortId PortId, DeviceId DeviceId, SubscriptionId SubscriptionId) : IMessage;
public readonly record struct SubscriptionRemovedMessage(PortId PortId, DeviceId DeviceId, SubscriptionId SubscriptionId) : IMessage;