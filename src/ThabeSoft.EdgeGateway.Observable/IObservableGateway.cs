using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Observable;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


public interface IObservableGateway : IGateway, IObservableLifecycleState
{
    //new IObservableDictionary<ConnectionEndpoint, IObservablePort> Ports { get; }
    new IObservableCollection<IObservablePort> Ports { get; }
}

public interface IObservablePort
{
    IObservable<IPort> PortAdded { get; }
    IObservable<IPort> PortRemoved { get; }
}

public interface IPortEventStream
{
    IObservable<PortAddedEvent> Added { get; }
    IObservable<PortRemovedEvent> Removed { get; }
    IObservable<PortUpdatedEvent> Updated { get; }
}

public readonly record struct PortAddedEvent(IPort Port);
public readonly record struct PortRemovedEvent(ConnectionEndpoint Endpoint);
public readonly record struct PortUpdatedEvent(IPort Port);