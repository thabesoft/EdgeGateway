using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines.Retryable;
using ThabeSoft.EdgeGateway.Lifecycles.Utilities;

namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines.Stric;


internal sealed class StrictLifecycleStateMachine(
        StrictLifecycleStateMachineDescriptor descriptor,
        ObservableLifecycleState lifecycleState,
        Executor executor,
        Locker locker,
        ILogger<StrictLifecycleStateMachine> logger,
        Func<ValueTask> onDispose
    ) : LifecycleStateMachineBase(lifecycleState, executor, locker, logger, onDispose)
{
    protected override TimeSpan StartTimeout => descriptor.StartTimeout;
    protected override TimeSpan StopTimeout => descriptor.StopTimeout;
    protected override TimeSpan TerminatedTimeout => descriptor.TerminatingTimeout;

    protected override bool CanTransitionToRunning() => State is LifecycleState.Created or LifecycleState.Stopped or LifecycleState.Faulted;
    protected override bool CanTransitionToStopped() => State is LifecycleState.Created or LifecycleState.Running or LifecycleState.Faulted;
    protected override bool CanTransitionToTerminated() => State is LifecycleState.Created or LifecycleState.Running or LifecycleState.Stopped or LifecycleState.Faulted;
}