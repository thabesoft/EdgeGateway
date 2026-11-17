using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines.Stric;

internal sealed class StrictLifecycleStateMachineFactory(ILoggerFactory loggerFactory) : ILifecycleStateMachineResolver
{
    public bool CanCreate(LifecycleStateMachineDescriptor descriptor)
    {
        return descriptor is StrictLifecycleStateMachineDescriptor;
    }

    public ILifecycleStateMachine Resolve(LifecycleStateMachineDescriptor descriptor)
    {
        if (descriptor is not StrictLifecycleStateMachineDescriptor strict)
            throw new InvalidOperationException($"不支持的描述符: {descriptor}");


        var state = new ObservableLifecycleState();
        var locker = new Locker(1, 1);
        var executor = new Executor(state, locker) { ExecuteTimeout = descriptor.ExecuteTimeout };
        var logger = loggerFactory.CreateLogger<StrictLifecycleStateMachine>();

        return new StrictLifecycleStateMachine(
                descriptor: strict,
                lifecycleState: state,
                executor: executor,
                locker: locker,
                logger: logger
            )
        {
            DisposedCallback = () =>
            {
                locker.Dispose();
                state.Dispose();

                return ValueTask.CompletedTask;
            }
        };
    }
}
