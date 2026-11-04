using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Options;


public interface IDeviceOptionsBuilder
{
    IDeviceOptionsBuilder SetDescriptor(IDeviceDescriptor descriptor);
    IDeviceOptionsBuilder AddTag(Action<ITagOptionsBuilder> buildAction);
}

public static class DeviceOptionsBuilderExtensions
{
    extension(IDeviceOptionsBuilder builder)
    {
        public IDeviceOptionsBuilder UseProtocol(DeviceId deviceId, IProtocolDescriptor descriptor) =>
            builder.SetDescriptor(DeviceDescriptor.Create(deviceId, descriptor));

        public IDeviceOptionsBuilder UseProtocol(IProtocolDescriptor descriptor) =>
            builder.SetDescriptor(DeviceDescriptor.Create(descriptor));

        public IDeviceOptionsBuilder AddTag(TagName tagName, ITagDescriptor descriptor, ITrigger trigger) =>
           builder.AddTag(x => x.WithDescriptor(descriptor).WithName(tagName).WithTrigger(trigger));
    }
}
