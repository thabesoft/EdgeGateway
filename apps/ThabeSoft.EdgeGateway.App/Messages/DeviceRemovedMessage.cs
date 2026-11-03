using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Devices;

namespace ThabeSoft.EdgeGateway.App.Messages;


public sealed record class DeviceRemovedMessage(ChannelAddress ChannelAddress, DeviceId DeviceId);