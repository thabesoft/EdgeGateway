using ThabeSoft.EdgeGateway.Devices;

namespace ThabeSoft.EdgeGateway.App.Port.Device;

public sealed record class DeviceCreatedMessage(DeviceId DeviceId);