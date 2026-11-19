using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ThabeSoft.EdgeGateway.Extensions;


public static class SafeDisposeExtensions
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


    /// <summary>
    /// 尝试识别并释放未知对象
    /// <para>返回值说明：</para>
    /// <list type="bullet">
    /// <item><c>true</c>：对象支持释放，释放成功</item>
    /// <item><c>false</c>：对象支持释放，但释放时发生异常（已记录日志）</item>
    /// <item><c>null</c>：对象不实现 IAsyncDisposable / IDisposable，无需释放</item>
    /// </list>
    /// </summary>
    public static async ValueTask<bool?> TryDisposeAsync(object? obj, ILogger? logger = null)
    {
        if (obj is IAsyncDisposable asyncDisposable)
            return await asyncDisposable.TryDisposeAsync(logger);

        if (obj is IDisposable disposable)
            return disposable.TryDispose(logger);

        return null;
    }

    /// <summary>
    /// 尝试识别并释放未知对象
    /// </summary>
    public static async ValueTask DisposeSafelyAsync(object? obj, ILogger? logger = null) =>
        await TryDisposeAsync(obj, logger);


    /// <summary>
    /// 安全释放所有对象
    /// </summary>
    public static async ValueTask<int> DisposeSafelyBatchAsync(IEnumerable<object> objects, ILogger? logger = null)
    {
        int count = 0;

        foreach (var obj in objects)
        {
            if (await TryDisposeAsync(obj, logger) == true)
                count++;
        }

        return count;
    }
}
