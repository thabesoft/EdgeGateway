#if NETSTANDARD2_0

namespace System.Threading;

public static class CancellationTokenSourceExtensions
{
    extension(CancellationTokenSource cts)
    {
        public Task CancelAsync()
        {
            return new Task(cts.Cancel);
        }
    }
}

#endif