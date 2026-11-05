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
        public IDeviceOptionsConfigurator UseProtocol(DeviceId deviceId, IProtocolDescriptor protocolDescriptor) =>
            builder.SetDescriptor(DeviceDescriptor.Create(deviceId, protocolDescriptor));

        public IDeviceOptionsConfigurator UseProtocol(IProtocolDescriptor protocolDescriptor) =>
            builder.SetDescriptor(DeviceDescriptor.Create(protocolDescriptor));

        public IDeviceOptionsConfigurator AddTag<T>(TagDescriptor<T> descriptor, ITriggerDescriptor triggerDescriptor) where T : notnull =>
           builder.AddTag<T>(x => x.WithDescriptor(descriptor).WithTrigger(triggerDescriptor));
    }
}
