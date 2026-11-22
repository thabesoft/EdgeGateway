using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.PubSub;

namespace ThabeSoft.EdgeGateway.Messages;

public readonly record struct DeviceStateChangedMessage(PortId PortId, DeviceId DeviceId, LifecycleState Old, LifecycleState New) : IMessage;
public readonly record struct DeviceAddedMessage(PortId PortId, DeviceId DeviceId) : IMessage;
public readonly record struct DeviceRemovedMessage(PortId PortId, DeviceId DeviceId) : IMessage;