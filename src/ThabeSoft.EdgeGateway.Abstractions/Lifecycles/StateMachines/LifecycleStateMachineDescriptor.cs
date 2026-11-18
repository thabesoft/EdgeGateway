using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines;


public abstract record class LifecycleStateMachineDescriptor
{
    public TimeSpan StartTimeout { get; init => field = value.Positive(); } = TimeSpan.FromSeconds(30);
    public TimeSpan StopTimeout { get; init => field = value.Positive(); } = TimeSpan.FromSeconds(15);
    public TimeSpan DisposedTimeout { get; init => field = value.Positive(); } = TimeSpan.FromSeconds(5);
    public TimeSpan ExecuteTimeout { get; init => field = value.Positive(); } = TimeSpan.FromSeconds(5);
}


public sealed record class RetryableLifecycleStateMachineDescriptor : LifecycleStateMachineDescriptor;
public sealed record class StrictLifecycleStateMachineDescriptor : LifecycleStateMachineDescriptor;