#if NETSTANDARD2_0

namespace System.Threading;

public static class CancellationTokenSourceExtensions
{
    extension(CancellationTokenSource cts)
    {
        public Task CancelAsync()
        {
            try { cts.Cancel(); }
            catch (AggregateException) { /* 回调异常，吞掉或记录 */ }
            return Task.CompletedTask;
        }
    }
}

#endif