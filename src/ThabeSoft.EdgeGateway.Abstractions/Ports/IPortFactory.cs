namespace ThabeSoft.EdgeGateway.Ports;


public interface IPortFactory
{
    bool CanCreate(IPortDescriptor descriptor);
    IPort Create(IPortDescriptor descriptor);
}