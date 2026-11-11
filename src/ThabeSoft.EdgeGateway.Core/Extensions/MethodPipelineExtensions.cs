using Microsoft.Extensions.Logging;

namespace ThabeSoft.EdgeGateway.Utilities;

internal static class MethodPipelineExtensions
{
    extension<TResult>(Func<CancellationToken, ValueTask<TResult>> action)
    {
        public Func<CancellationToken, ValueTask<TResult>> WithLog(ILogger logger, string operation)
        {
            return async ct =>
            {
                try
                {
                    var result = await action(ct);
                    logger.LogDebug("{Operation} 成功", operation);
                    return result;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "{Operation} 失败", operation);
                    throw;
                }
            };
        }
    }
}
