using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Options;


public interface IDeviceOptionsBuilder
{
    IDeviceOptionsBuilder SetDescriptor(DeviceDescriptor descriptor);
    IDeviceOptionsBuilder AddTag<T>(Action<ITagOptionsBuilder<T>> buildAction) where T : notnull;
}

public static class DeviceOptionsBuilderExtensions
{
    extension(IDeviceOptionsBuilder builder)
    {
        public IDeviceOptionsBuilder UseProtocol(DeviceId deviceId, IProtocolDescriptor descriptor) =>
            builder.SetDescriptor(DeviceDescriptor.Create(deviceId, descriptor));

        public IDeviceOptionsBuilder UseProtocol(IProtocolDescriptor descriptor) =>
            builder.SetDescriptor(DeviceDescriptor.Create(descriptor));

        public IDeviceOptionsBuilder AddTag<T>(ITagDescriptor<T> descriptor, ITrigger trigger) where T : notnull =>
           builder.AddTag<T>(x => x.WithDescriptor(descriptor).WithTrigger(trigger));
    }
}
