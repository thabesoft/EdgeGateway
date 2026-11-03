namespace ThabeSoft.EdgeGateway.Options;

public interface IGatewayOptionsBuilder
{
    IGatewayOptionsBuilder AddPort(Action<IPortOptionsBuilder> buildAction);
}