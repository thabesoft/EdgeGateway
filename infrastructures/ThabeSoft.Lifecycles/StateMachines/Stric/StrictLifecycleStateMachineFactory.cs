using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles.Utilities;
using ThabeSoft.Primitives;

namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines.Stric;


internal sealed class StrictLifecycleStateMachineFactory(ILoggerFactory loggerFactory) : ILifecycleStateMachineResolver
{
    public bool CanResolve(LifecycleStateMachineDescriptor descriptor)
    {
        return descriptor is StrictLifecycleStateMachineDescriptor;
    }

    public ILifecycleStateMachine Resolve(LifecycleStateMachineDescriptor descriptor)
    {
        if (descriptor is not StrictLifecycleStateMachineDescriptor strict)
            throw new InvalidOperationException($"不支持的描述符: {descriptor}");


        var logger = loggerFactory.CreateLogger<StrictLifecycleStateMachine>();
        var stateLogger = loggerFactory.CreateLogger<ObservableLifecycleState>();

        var state = new ObservableLifecycleState(stateLogger);
        var locker = new Locker(1, 1) { Timeout = descriptor.TransitionWaitTimeout };
        var executor = new Executor(state, locker) { ExecuteTimeout = descriptor.ExecuteTimeout };


        return new StrictLifecycleStateMachine
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
