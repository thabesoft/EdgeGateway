using ThabeSoft.EdgeGateway.Pipelines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.StateMachines;

namespace ThabeSoft.EdgeGateway.Extensions;


public static class LoggedMethodPipelineExtensions
{
    extension<TResult>(AsyncPipelineHandler<TResult> action)
    {
        public AsyncPipelineHandler<TResult> WithExecutor(IAsyncExecutor executor)
        {
            return ct => executor.ExecuteAsync(ct => action(ct), ct);
        }
    }
}