using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines.Retryable;


internal sealed class RetryableLifecycleStateMachineFactory(ILoggerFactory loggerFactory) : ILifecycleStateMachineResolver
{
    public bool CanCreate(LifecycleStateMachineDescriptor descriptor)
    {
        return descriptor is RetryableLifecycleStateMachineDescriptor;
    }

    public ILifecycleStateMachine Create(LifecycleStateMachineDescriptor descriptor)
    {
        if (descriptor is not RetryableLifecycleStateMachineDescriptor strict)
            throw new InvalidOperationException($"不支持的描述符: {descriptor}");


        var state = new ObservableLifecycleState();
        var locker = new Locker(1, 1);
        var executor = new Executor(state, locker) { ExecuteTimeout = descriptor.ExecuteTimeout };
        var logger = loggerFactory.CreateLogger<RetryableLifecycleStateMachine>();

        return new RetryableLifecycleStateMachine(
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
