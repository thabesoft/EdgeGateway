using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.App.Port.Device;


public sealed record class DeviceRemovedMessage(PortId PortId, DeviceId DeviceId);