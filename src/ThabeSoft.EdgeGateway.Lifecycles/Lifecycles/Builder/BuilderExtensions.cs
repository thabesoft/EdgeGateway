using ThabeSoft.EdgeGateway.Lifecycles.Builder;

namespace ThabeSoft.EdgeGateway.Lifecycles;

public static class BuilderExtensions
{
    extension<T>(T builder) where T : INormalLifecycleTimeoutBuilder<T>
    {
        public T Start(double timeoutSeconds) =>
            builder.Start(TimeSpan.FromSeconds(timeoutSeconds));
        public T Stop(double timeoutSeconds) =>
            builder.Stop(TimeSpan.FromSeconds(timeoutSeconds));
        public T Dispose(double timeoutSeconds) =>
            builder.Dispose(TimeSpan.FromSeconds(timeoutSeconds));
        public T Lock(double timeoutSeconds) =>
            builder.Lock(TimeSpan.FromSeconds(timeoutSeconds));


        public T StartStop(TimeSpan timeout) =>
            builder.Start(timeout).Stop(timeout);
        public T StartStop(double timeoutSeconds) =>
            builder.StartStop(TimeSpan.FromSeconds(timeoutSeconds));


        public T StartStopDispose(TimeSpan timeout) =>
             builder.StartStop(timeout).Dispose(timeout);
        public T StartStopDispose(double timeoutSeconds) =>
            builder.StartStopDispose(TimeSpan.FromSeconds(timeoutSeconds));


        public T StartStopDisposeLock(TimeSpan timeout) =>
            builder.StartStopDispose(timeout).Lock(timeout);
        public T StartStopDisposeLock(double timeoutSeconds) =>
            builder.StartStopDisposeLock(TimeSpan.FromSeconds(timeoutSeconds));
    }
}
