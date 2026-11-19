using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles.Utilities;
using ThabeSoft.EdgeGateway.Primitives;

namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines.Retryable;


internal sealed class RetryableLifecycleStateMachineResolver(
        ILoggerFactory loggerFactory
    ) : ILifecycleStateMachineResolver
{
    public bool CanResolve(LifecycleStateMachineDescriptor descriptor)
    {
        return descriptor is RetryableLifecycleStateMachineDescriptor;
    }

    public ILifecycleStateMachine Resolve(LifecycleStateMachineDescriptor descriptor)
    {
        if (descriptor is not RetryableLifecycleStateMachineDescriptor strict)
            throw new InvalidOperationException($"不支持的描述符: {descriptor}");


        var logger = loggerFactory.CreateLogger<RetryableLifecycleStateMachine>();
        var stateLogger = loggerFactory.CreateLogger<ObservableLifecycleState>();

        var state = new ObservableLifecycleState(stateLogger);
        var locker = new Locker(1, 1) { Timeout = descriptor.TransitionWaitTimeout };
        var executor = new Executor(state, locker) { ExecuteTimeout = descriptor.ExecuteTimeout };


        return new RetryableLifecycleStateMachine
        (
            descriptor: strict,
            lifecycleState: state,
            executor: executor,
            locker: locker,
            logger: logger,
            onDispose: async () =>
            {
                locker.DisposeSafely(logger);
                state.DisposeSafely(stateLogger);
            }
        );
    }
}
