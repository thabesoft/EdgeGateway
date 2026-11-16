namespace ThabeSoft.EdgeGateway.Lifecycles;


public interface ILifecycleStateNotifier
{
    IObservable<LifecycleStateTransition> StateChanged { get; }
}

public readonly record struct LifecycleStateTransition(LifecycleState Old, LifecycleState New);
