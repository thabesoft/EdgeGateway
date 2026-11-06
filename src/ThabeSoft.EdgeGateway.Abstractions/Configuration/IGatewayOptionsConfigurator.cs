using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Configuration;


public interface IGatewayOptionsConfigurator
{
    IGatewayOptionsConfigurator WithAutoLaunch(bool value);
    IGatewayOptionsConfigurator AddPort(Action<IPortOptionsConfigurator> buildAction);
}


public static class GatewayOptionsConfiguratorExtensions
{
    extension(IGatewayOptionsConfigurator builder)
    {
        public IGatewayOptionsConfigurator EnableAutoLaunch() =>
            builder.WithAutoLaunch(true);

        public IGatewayOptionsConfigurator DisabledAutoLaunch() =>
            builder.WithAutoLaunch(false);
    }
}




public interface IGatewayOptionsConfiguratorV2
{
    IPortOptionsConfiguratorV2 AddPort(ITransportDescriptor transport);
}
public interface IPortOptionsConfiguratorV2 : IGatewayOptionsConfiguratorV2
{
    IDeviceOptionsConfiguratorV2 AddDevice(DeviceId deviceId, IProtocolDescriptor protocol);
}
public interface IDeviceOptionsConfiguratorV2 : IPortOptionsConfiguratorV2
{
    ITagOptionsConfiguratorV2<T> AddSubscription<T>(TagDescriptor<T> tagDescriptor, ITriggerDescriptor triggerDescriptor) where T : notnull;
}
public interface ITagOptionsConfiguratorV2<T> : IDeviceOptionsConfiguratorV2
    where T : notnull
{
    ITagOptionsConfiguratorV2<T> OnValue(Action<T> action);
}

internal delegate void Op(IGatewayOptionsConfiguratorV2 action);