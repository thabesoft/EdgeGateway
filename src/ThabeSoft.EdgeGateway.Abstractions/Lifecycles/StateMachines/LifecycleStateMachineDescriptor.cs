using ThabeSoft.EdgeGateway.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines;


public abstract record class LifecycleStateMachineDescriptor
{
    /// <summary>
    /// 启动最大耗时
    /// </summary>
    public TimeSpan StartTimeout { get; init => field = value.Positive(); } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 停止最大耗时
    /// </summary>
    public TimeSpan StopTimeout { get; init => field = value.Positive(); } = TimeSpan.FromSeconds(15);

    /// <summary>
    /// 终止最大耗时
    /// </summary>
    public TimeSpan TerminatingTimeout { get; init => field = value.Positive(); } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// 执行方法最大耗时
    /// </summary>
    public TimeSpan ExecuteTimeout { get; init => field = value.Positive(); } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// 状态切换最大时长
    /// </summary>
    public TimeSpan TransitionWaitTimeout { get; init => field = value.Positive(); } = TimeSpan.FromSeconds(5);
}


public sealed record class RetryableLifecycleStateMachineDescriptor : LifecycleStateMachineDescriptor;
public sealed record class StrictLifecycleStateMachineDescriptor : LifecycleStateMachineDescriptor;