namespace ThabeSoft.EdgeGateway.Port;


public interface IPortFactory
{
    bool CanCreate(IPortDescriptor descriptor);
    IPort Create(IPortDescriptor descriptor);
}