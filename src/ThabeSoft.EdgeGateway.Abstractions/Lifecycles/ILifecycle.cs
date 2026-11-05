namespace ThabeSoft.EdgeGateway.Lifecycles;

public interface ILifecycle 
{
    LifecycleState State { get; }

    ValueTask StartAsync(CancellationToken cancellationToken = default);
    ValueTask StopAsync(CancellationToken cancellationToken = default);
}

public static class LifecycleExtensions
{
    extension(ILifecycle lifecycle)
    {
        public bool IsRunning => lifecycle.State == LifecycleState.Running;
        public bool IsStopped => lifecycle.State == LifecycleState.Stopped;
        public bool IsFaulted => lifecycle.State == LifecycleState.Faulted;
    }
}