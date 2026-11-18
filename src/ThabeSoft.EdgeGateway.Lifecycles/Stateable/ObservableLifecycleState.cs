using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Stateable;


internal sealed class ObservableLifecycleState : ILifecycleStateable, ILifecycleStateNotifier, IDisposable
{
    private readonly Subject<LifecycleStateTransition> _stateSubject = new();


    public LifecycleState State { get; private set; }
    public IObservable<LifecycleStateTransition> StateChanged => _stateSubject;


    public bool ChangeState(LifecycleState @new)
    {
        if (State == @new) return false;

        var old = State;
        State = @new;
        _stateSubject.OnNext(new(old, @new));

        return true;
    }


    public bool OnStarting() => ChangeState(LifecycleState.Starting);
    public bool OnRunning() => ChangeState(LifecycleState.Running);
    public bool OnStopping() => ChangeState(LifecycleState.Stopping);
    public bool OnStopped() => ChangeState(LifecycleState.Stopped);
    public bool OnDisposed() => ChangeState(LifecycleState.Disposed);
    public bool OnFaulted() => ChangeState(LifecycleState.Faulted);



    public void Dispose()
    {
        _stateSubject.Dispose();
    }
}