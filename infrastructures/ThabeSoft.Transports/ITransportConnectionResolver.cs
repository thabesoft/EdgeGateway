namespace ThabeSoft.Transports;


public interface ITransportConnectionResolver
{
    bool CanResolve(TransportDescriptor descriptor);
    ITransportConnection Resolve(TransportDescriptor descriptor);
}