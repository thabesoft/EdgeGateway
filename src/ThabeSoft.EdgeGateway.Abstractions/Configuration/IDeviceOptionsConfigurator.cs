using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Configuration;


public interface IDeviceOptionsConfigurator
{
    IDeviceOptionsConfigurator SetDescriptor(DeviceDescriptor descriptor);
    IDeviceOptionsConfigurator AddSubscription<T>(Action<ISubscriptionOptionsConfigurator<T>> buildAction) where T : notnull;
}

public static class DeviceOptionsConfiguratorExtensions
{
    extension(IDeviceOptionsConfigurator builder)
    {
        public IDeviceOptionsConfigurator UseProtocol(DeviceId deviceId, IProtocolDescriptor protocolDescriptor) =>
            builder.SetDescriptor(DeviceDescriptor.Create(deviceId, protocolDescriptor));

        public IDeviceOptionsConfigurator UseProtocol(IProtocolDescriptor protocolDescriptor) =>
            builder.SetDescriptor(DeviceDescriptor.Create(protocolDescriptor));


        public IDeviceOptionsConfigurator AddSubscription<T>(TagDescriptor<T> descriptor, ITriggerDescriptor triggerDescriptor, params Action<T>[] tagUpdatee) where T : notnull =>
           builder.AddSubscription<T>(x =>
           {
               x.WithDescriptor(SubscriptionDescriptor.Create(descriptor, triggerDescriptor));

               if (tagUpdatee.Length == 0) return;
               foreach (var i in tagUpdatee)
                   x.AddTagCallback(i);
           });
    }
}