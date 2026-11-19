using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace ThabeSoft.EdgeGateway.Primitives;


public interface IDisposeMarker
{
    bool IsDisposed { get; }
    bool TryMarkDisposed();
    void ThrowIfDisposed(object obj);
}


/// <summary>
/// 释放标记
/// </summary>
public struct DisposeFlag : IDisposeMarker
{
    private int _disposed;

    /// <summary>
    /// 是否已经释放
    /// </summary>
    public bool IsDisposed => Volatile.Read(ref _disposed) == 1;

    /// <summary>
    /// 尝试标记为已释放  (幂等操作, 仅能成功一次, 多线程安全)
    /// </summary>
    public bool TryMarkDisposed() => Interlocked.CompareExchange(ref _disposed, 1, 0) == 0;

    /// <summary>
    /// 如果释放了则抛异常
    /// </summary>
    /// <exception cref="ObjectDisposedException" />
#if !NETSTANDARD2_0
    [StackTraceHidden]
#endif
    public void ThrowIfDisposed(object obj) => ObjectDisposedException.ThrowIf(IsDisposed, obj);
}


/// <summary>
/// 异步释放令牌：持有一段异步清理回调，幂等执行，支持查询释放状态
/// </summary>
[Obsolete]
public readonly struct AsyncDisposableToken(Func<ValueTask> onDispose, ILogger? logger = null) : IAsyncDisposable, IDisposeMarker
{
    private readonly DisposeFlag _flag = new();

    public bool IsDisposed => _flag.IsDisposed;

#if !NETSTANDARD2_0
    [StackTraceHidden]
#endif
    public void ThrowIfDisposed(object obj) => _flag.ThrowIfDisposed(obj);


    public bool TryMarkDisposed() => _flag.TryMarkDisposed();

    /// <summary>
    /// 幂等释放, 并安全回调
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (!_flag.TryMarkDisposed()) return;
        if (onDispose is null) return;

        try
        {
            await onDispose.Invoke();
        }
        catch(Exception ex)
        {
            (logger ?? NullLogger.Instance).LogError(ex, "释放回调执行失败");
        }
    }

}