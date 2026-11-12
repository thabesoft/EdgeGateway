using System.ComponentModel;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.Subscription;

public interface IObservablSubscription : ISubscription, IObservableLifecycleState;