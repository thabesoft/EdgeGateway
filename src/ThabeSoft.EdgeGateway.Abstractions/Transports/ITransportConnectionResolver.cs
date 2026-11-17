namespace ThabeSoft.EdgeGateway.Transports;


public interface ITransportConnectionResolver
{
    bool CanResolve(TransportCreationContext context);
    ITransportConnection Resolve(TransportCreationContext context);
}