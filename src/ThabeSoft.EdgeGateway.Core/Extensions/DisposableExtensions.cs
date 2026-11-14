using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


internal static class DisposableExtensions
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
                (logger ?? NullLogger.Instance).LogError(ex, "设备释放失败");
                return false;
            }
        }
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
                (logger ?? NullLogger.Instance).LogError(ex, "设备释放失败");
                return false;
            }
        }
    }
}
