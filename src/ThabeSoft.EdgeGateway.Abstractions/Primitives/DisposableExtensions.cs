using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ThabeSoft.EdgeGateway.Primitives;


public static class DisposableExtensions
{
    extension<T>(T obj) where T : IAsyncDisposable
    {
        public async ValueTask<bool> TryDisposeAsync(ILogger? logger = null)
        {
            try
            {
                await obj.DisposeAsync();
                return true;
            }
            catch (Exception ex)
            {
                (logger ?? NullLogger.Instance).LogError(ex, "资源释放失败");
                return false;
            }
        }

        public async ValueTask DisposeSafelyAsync(ILogger? logger = null) =>
           await obj.TryDisposeAsync(logger);
    }

    extension<T>(T obj) where T : IDisposable
    {
        public bool TryDispose(ILogger? logger = null)
        {
            try
            {
                obj.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                (logger ?? NullLogger.Instance).LogError(ex, "资源释放失败");
                return false;
            }
        }

        public void DisposeSafely(ILogger? logger = null) =>
            obj.TryDispose(logger);
    }
}