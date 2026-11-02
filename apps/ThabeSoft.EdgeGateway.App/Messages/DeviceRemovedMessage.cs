using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.App.Messages;


public sealed record class DeviceRemovedMessage(PortId PortId, DeviceId DeviceId);