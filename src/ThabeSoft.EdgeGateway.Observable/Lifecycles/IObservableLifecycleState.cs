namespace ThabeSoft.EdgeGateway.Lifecycles;


public interface IObservableLifecycleState
{
    IObservable<LifecycleStateChanged> StateChanged { get; }
}

public readonly record struct LifecycleStateChanged(LifecycleState OldState, LifecycleState NewState);