#if NETSTANDARD2_0

namespace System.Threading.Tasks;

/// <summary>
/// netstandard2.0 下无泛型 TaskCompletionSource 的兼容替代。
/// 行为对齐 .NET 5+ 的 TaskCompletionSource（非泛型），
/// 内部用 TaskCompletionSource&lt;bool&gt; 承载，结果值被丢弃。
/// </summary>
public sealed class TaskCompletionSource(object? state, TaskCreationOptions creationOptions)
{
    private readonly TaskCompletionSource<bool> _inner = new(state, creationOptions);


    public TaskCompletionSource() : this(TaskCreationOptions.None) { }
    public TaskCompletionSource(TaskCreationOptions creationOptions) : this(null, creationOptions) { }
    public TaskCompletionSource(object? state) : this(state, TaskCreationOptions.None) { }


    public Task Task => _inner.Task;


    public void SetResult() => _inner.SetResult(true);
    public void SetCanceled() => _inner.SetCanceled();

    public void SetException(Exception exception) => _inner.SetException(exception);
    public void SetException(IEnumerable<Exception> exceptions) => _inner.SetException(exceptions);

    public bool TrySetResult() => _inner.TrySetResult(true);
    public bool TrySetCanceled() => _inner.TrySetCanceled();
    public bool TrySetCanceled(CancellationToken cancellationToken) => _inner.TrySetCanceled(cancellationToken);
    public bool TrySetException(Exception exception) => _inner.TrySetException(exception);
    public bool TrySetException(IEnumerable<Exception> exceptions) => _inner.TrySetException(exceptions);
}

#endif