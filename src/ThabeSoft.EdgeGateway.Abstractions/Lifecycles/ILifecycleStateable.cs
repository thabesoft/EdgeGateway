namespace ThabeSoft.EdgeGateway.Lifecycles;

public interface ILifecycleStateable
{
    LifecycleState State { get; }
}

public static class LifecycleExtensions
{
    extension(ILifecycleStateable lifecycle)
    {
        public bool IsRunning => lifecycle.State == LifecycleState.Running;
        public bool IsStopped => lifecycle.State == LifecycleState.Stopped;
        public bool IsFaulted => lifecycle.State == LifecycleState.Faulted;
    }
}