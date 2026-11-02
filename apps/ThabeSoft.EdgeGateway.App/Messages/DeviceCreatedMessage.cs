using ThabeSoft.EdgeGateway.Devices;

namespace ThabeSoft.EdgeGateway.App.Messages;

public sealed record class DeviceCreatedMessage(DeviceId DeviceId);