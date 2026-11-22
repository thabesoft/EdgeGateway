using System.Diagnostics;

namespace ThabeSoft.Primitives;


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