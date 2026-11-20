namespace ThabeSoft.EdgeGateway.Lifecycles;


/// <summary>
/// 生命周期状态
/// </summary>
public enum LifecycleState : byte
{
    /// <summary>从未启动</summary>
    Created,

    /// <summary>操作失败</summary>
    Faulted,


    /// <summary>正在启动中</summary>
    Starting,

    /// <summary>正在运行中</summary>
    Running,


    /// <summary>正在停止中</summary>
    Stopping,

    /// <summary>已停止</summary>
    Stopped,


    /// <summary> 终止中 </summary>
    Terminating,

    /// <summary>已终止</summary>
    Terminated
}