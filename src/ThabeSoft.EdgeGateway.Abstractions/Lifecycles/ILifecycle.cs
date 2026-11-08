namespace ThabeSoft.EdgeGateway.Lifecycles;

public interface ILifecycle : ILifecycleState
{
    ValueTask StartAsync(CancellationToken cancellationToken = default);
    ValueTask StopAsync(CancellationToken cancellationToken = default);
}

public interface ILifecycleState
{
    LifecycleState State { get; }
}


public static class LifecycleExtensions
{
    extension(ILifecycleState lifecycle)
    {
        public bool IsRunning => lifecycle.State == LifecycleState.Running;
        public bool IsStopped => lifecycle.State == LifecycleState.Stopped;
        public bool IsFaulted => lifecycle.State == LifecycleState.Faulted;
    }
}