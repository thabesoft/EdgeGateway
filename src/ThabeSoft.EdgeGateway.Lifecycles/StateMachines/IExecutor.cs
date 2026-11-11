namespace ThabeSoft.EdgeGateway.StateMachines;


/// <summary>
/// 同步执行器
/// </summary>
public interface IExecutor
{
    void Execute(Action action);
    TResult Execute<TResult>(Func<TResult> action);
}