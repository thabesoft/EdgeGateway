using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Events;


public interface IEventPublisher
{
    void Publish<TEvent>(TEvent @event) where TEvent : IEvent;
}


public record class PortRemoved(PortId PortId);
public record class PortAdded(IPort Port);
public record class DeviceRemoved(PortId PortId, DeviceId DeviceId);
public record class DeviceAdded(PortId PortId, IDevice Device);