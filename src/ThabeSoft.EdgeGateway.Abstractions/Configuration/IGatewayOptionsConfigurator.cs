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