using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Observable;

namespace ThabeSoft.EdgeGateway.Ports;

public interface IObservablePort : IPort, IObservableLifecycleState
{
    new IObservableDictionary<DeviceId, IObservableDevice> Devices { get; }
}