using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ThabeSoft.Primitives;


public static class Safe
{
    public static async ValueTask<bool> TryEexecuteAsync(Func<ValueTask> handler, ILogger? logger = null, string errorMessage = null)
    {
        try
        {
            await handler.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            (logger ?? NullLogger.Instance).LogError(ex, errorMessage ?? "执行失败");
            return false;
        }
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
    public static async ValueTask DisposeSafelyAsync(object? obj, ILogger? logger = null)
    {

        await TryDisposeAsync(obj, logger);
    }

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