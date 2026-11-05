using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Configuration;


public interface IDeviceOptionsConfigurator
{
    IDeviceOptionsConfigurator SetDescriptor(DeviceDescriptor descriptor);
    IDeviceOptionsConfigurator AddTag<T>(Action<ITagOptionsConfigurator<T>> buildAction) where T : notnull;
}

public static class DeviceOptionsConfiguratorExtensions
{
    extension(IDeviceOptionsConfigurator builder)
    {
        public IDeviceOptionsConfigurator UseProtocol(DeviceId deviceId, IProtocolDescriptor descriptor) =>
            builder.SetDescriptor(DeviceDescriptor.Create(deviceId, descriptor));

        public IDeviceOptionsConfigurator UseProtocol(IProtocolDescriptor descriptor) =>
            builder.SetDescriptor(DeviceDescriptor.Create(descriptor));

        public IDeviceOptionsConfigurator AddTag<T>(TagDescriptor<T> descriptor, ITrigger trigger) where T : notnull =>
           builder.AddTag<T>(x => x.WithDescriptor(descriptor).WithTrigger(trigger));
    }
}
