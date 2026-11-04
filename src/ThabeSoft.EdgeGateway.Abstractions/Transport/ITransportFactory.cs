namespace ThabeSoft.EdgeGateway.Transport;


public interface ITransportFactory
{
    bool CanCreate(ITransportDescriptor descriptor);
    ITransportConnection Create(ITransportDescriptor descriptor);
}