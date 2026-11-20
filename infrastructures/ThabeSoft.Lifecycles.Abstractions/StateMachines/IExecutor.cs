namespace ThabeSoft.Lifecycles.StateMachines;


/// <summary>
/// 同步执行器
/// </summary>
public interface IExecutor
{
    void Execute(Action handler);
    TResult Execute<TResult>(Func<TResult> handler);
}