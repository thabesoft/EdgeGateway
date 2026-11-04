namespace ThabeSoft.EdgeGateway.Options;

public interface IGatewayOptionsBuilder
{
    IGatewayOptionsBuilder WithAutoLaunch(bool value);
    IGatewayOptionsBuilder AddPort(Action<IPortOptionsBuilder> buildAction);
}


public static class GatewayOptionsBuilderExtensions
{
    extension(IGatewayOptionsBuilder builder)
    {
        public IGatewayOptionsBuilder EnableAutoLaunch() =>
            builder.WithAutoLaunch(true);

        public IGatewayOptionsBuilder DisabledAutoLaunch() =>
            builder.WithAutoLaunch(false);
    }
}