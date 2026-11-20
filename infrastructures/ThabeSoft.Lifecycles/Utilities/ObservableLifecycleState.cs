using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ThabeSoft.Primitives;

namespace ThabeSoft.Lifecycles.Utilities;


internal sealed class ObservableLifecycleState(
        ILogger<ObservableLifecycleState>? logger
    ) : ILifecycleStateable, ILifecycleStateNotifier, IDisposable
{
    private ILogger<ObservableLifecycleState> _logger = logger ?? NullLogger<ObservableLifecycleState>.Instance;
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
    public bool OnTerminated() => ChangeState(LifecycleState.Terminated);
    public bool OnTerminating() => ChangeState(LifecycleState.Terminating);
    public bool OnFaulted() => ChangeState(LifecycleState.Faulted);


    public void Dispose() => _stateSubject.TryDispose(_logger);
}